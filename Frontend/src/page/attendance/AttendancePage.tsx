import { useCallback, useMemo, useState, useEffect } from 'react';
import {
    CCard,
    CCardBody,
    CCardHeader,
    CRow,
    CCol,
    CFormSelect,
    CFormInput,
    CButton,
    CTable,
    CTableHead,
    CTableRow,
    CTableHeaderCell,
    CTableBody,
    CTableDataCell,
    CAvatar,
    CSpinner
} from '@coreui/react';
import { CChart } from '@coreui/react-chartjs';
import { Save, Download, Search } from 'lucide-react';
import {
    getFaculties,
    getPrograms,
    getCourses,
    getAttendance,
    saveAttendance,
    exportAttendance,
    getAttendanceStats
} from '../../service/attendanceService';
import type {
    Faculty,
    Program,
    Course,
    AttendanceRecord,
    AttendanceStatus,
    AttendanceStats
} from '../../models/attendance/Attendance.types';

const DOUGHNUT_CANVAS_SIZE = 300;
const DOUGHNUT_CONTAINER_SIZE = 320;
const DOUGHNUT_COLORS = ['#2eb85c', '#e55353', '#f9b115'] as const;

export default function AttendancePage() {
    const [faculties, setFaculties] = useState<Faculty[]>([]);
    const [programs, setPrograms] = useState<Program[]>([]);
    const [courses, setCourses] = useState<Course[]>([]);

    const [selectedFaculty, setSelectedFaculty] = useState<string>('');
    const [selectedProgram, setSelectedProgram] = useState<string>('');
    const [selectedCourse, setSelectedCourse] = useState<string>('');
    const [selectedDate, setSelectedDate] = useState<string>(new Date().toISOString().split('T')[0]);

    const [chartMonth, setChartMonth] = useState<string>(new Date().toISOString().slice(0, 7)); // YYYY-MM
    const [chartCourse, setChartCourse] = useState<string>('');
    const [attendanceStats, setAttendanceStats] = useState<AttendanceStats | null>(null);
    const [loadingStats, setLoadingStats] = useState<boolean>(false);

    const courseOptions = useMemo(
        () =>
            courses.map((c) => (
                <option key={c.id} value={c.id}>
                    {c.name} ({c.code})
                </option>
            )),
        [courses],
    );

    const doughnutContainerStyle = useMemo(
        () => ({
            position: 'relative' as const,
            height: `${DOUGHNUT_CONTAINER_SIZE}px`,
            width: `${DOUGHNUT_CONTAINER_SIZE}px`,
            maxWidth: '100%',
            margin: '0 auto',
            overflow: 'hidden',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
        }),
        [],
    );

    const doughnutOptions = useMemo(
        () =>
            ({
                cutout: '55%',
                maintainAspectRatio: true,
                responsive: false,
                interaction: {
                    mode: 'nearest',
                    intersect: true,
                },
                plugins: {
                    legend: {
                        position: 'bottom',
                    },
                    tooltip: {
                        enabled: true,
                        callbacks: {
                            label: (context: any) => {
                                const label = context.label || '';
                                const value = Number(context.raw ?? 0);
                                const dataset = context.dataset;
                                const total = Array.isArray(dataset?.data)
                                    ? dataset.data.reduce((acc: number, current: number) => acc + Number(current ?? 0), 0)
                                    : 0;
                                const percentage = total > 0 ? Math.round((value / total) * 100) : 0;
                                return `${label}: ${value} (${percentage}%)`;
                            },
                        },
                    },
                },
                animation: false,
                animations: {
                    radius: false,
                    rotation: false,
                },
            }) as any,
        [],
    );

    const doughnutData = useMemo(() => {
        if (!attendanceStats) return null;
        return {
            labels: ['Present', 'Absent', 'Late'],
            datasets: [
                {
                    backgroundColor: DOUGHNUT_COLORS,
                    data: [attendanceStats.present, attendanceStats.absent, attendanceStats.late],
                    hoverOffset: 4,
                    borderWidth: 1,
                    hoverBorderWidth: 1,
                },
            ],
        };
    }, [attendanceStats]);

    const [attendanceRecords, setAttendanceRecords] = useState<AttendanceRecord[]>([]);
    const [loading, setLoading] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    useEffect(() => {
        void (async () => {
            const data = await getFaculties();
            setFaculties(data);
        })();
    }, []);

    useEffect(() => {
        if (!selectedFaculty) {
            setPrograms([]);
            setCourses([]);
            return;
        }

        void (async () => {
            const data = await getPrograms(selectedFaculty);
            setPrograms(data);
            setSelectedProgram('');
            setCourses([]);
            setSelectedCourse('');
        })();
    }, [selectedFaculty]);

    useEffect(() => {
        if (!selectedProgram) {
            setCourses([]);
            return;
        }

        void (async () => {
            const data = await getCourses(selectedProgram);
            setCourses(data);
            setSelectedCourse('');
        })();
    }, [selectedProgram]);

    useEffect(() => {
        if (!chartCourse || !chartMonth) {
            setAttendanceStats(null);
            return;
        }

        void (async () => {
            setLoadingStats(true);
            try {
                const stats = await getAttendanceStats(chartCourse, chartMonth);
                setAttendanceStats(stats);
            } catch (error) {
                console.error('Failed to fetch stats', error);
            } finally {
                setLoadingStats(false);
            }
        })();
    }, [chartCourse, chartMonth]);

    useEffect(() => {
        if (selectedCourse && !chartCourse) {
            setChartCourse(selectedCourse);
        }
    }, [selectedCourse, chartCourse]);

    const handleSearch = useCallback(async () => {
        if (!selectedCourse || !selectedDate) {
            alert('Please select a course and date');
            return;
        }
        setLoading(true);
        try {
            const data = await getAttendance(selectedCourse, selectedDate);
            setAttendanceRecords(data);
        } catch (error) {
            console.error('Failed to fetch attendance', error);
        } finally {
            setLoading(false);
        }
    }, [selectedCourse, selectedDate]);

    const handleStatusChange = useCallback((id: number, status: AttendanceStatus) => {
        setAttendanceRecords(prev => prev.map(record =>
            record.id === id ? { ...record, status } : record
        ));
    }, []);

    const handleNoteChange = useCallback((id: number, note: string) => {
        setAttendanceRecords(prev => prev.map(record =>
            record.id === id ? { ...record, note } : record
        ));
    }, []);

    const handleSave = useCallback(async () => {
        setSaving(true);
        try {
            await saveAttendance(attendanceRecords);
            alert('Attendance saved successfully!');
        } catch (error) {
            console.error('Failed to save attendance', error);
            alert('Failed to save attendance.');
        } finally {
            setSaving(false);
        }
    }, [attendanceRecords]);

    const handleExport = useCallback(async () => {
        if (!selectedCourse || !selectedDate) return;
        try {
            await exportAttendance(selectedCourse, selectedDate);
            alert('Attendance exported successfully!');
        } catch (error) {
            console.error('Failed to export attendance', error);
            alert('Failed to export attendance.');
        }
    }, [selectedCourse, selectedDate]);

    const attendanceStatusButtons = useMemo(
        () =>
            [
                { label: 'Present', color: 'success', status: 'Present' as const },
                { label: 'Absent', color: 'danger', status: 'Absent' as const },
                { label: 'Late', color: 'warning', status: 'Late' as const },
            ],
        [],
    );

    return (
        <div className="p-4">
            <h2 className="mb-4">Attendance Management</h2>
            <CCard className="mb-4">
                <CCardHeader>
                    <strong>Subject Attendance</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow className="g-3">
                        <CCol md={3}>
                            <CFormSelect
                                label="Faculty"
                                value={selectedFaculty}
                                onChange={(e) => setSelectedFaculty(e.target.value)}
                            >
                                <option value="">Select Faculty</option>
                                {faculties.map(f => (
                                    <option key={f.id} value={f.id}>{f.name}</option>
                                ))}
                            </CFormSelect>
                        </CCol>
                        <CCol md={3}>
                            <CFormSelect
                                label="Program"
                                value={selectedProgram}
                                onChange={(e) => setSelectedProgram(e.target.value)}
                                disabled={!selectedFaculty}
                            >
                                <option value="">Select Program</option>
                                {programs.map(p => (
                                    <option key={p.id} value={p.id}>{p.name}</option>
                                ))}
                            </CFormSelect>
                        </CCol>
                        <CCol md={3}>
                            <CFormSelect
                                label="Course"
                                value={selectedCourse}
                                onChange={(e) => setSelectedCourse(e.target.value)}
                                disabled={!selectedProgram}
                            >
                                <option value="">Select Course</option>
                                {courseOptions}
                            </CFormSelect>
                        </CCol>
                        <CCol md={2}>
                            <CFormInput
                                type="date"
                                label="Lecture Date"
                                value={selectedDate}
                                onChange={(e) => setSelectedDate(e.target.value)}
                            />
                        </CCol>
                        <CCol md={1} className="d-flex align-items-end">
                            <CButton color="primary" onClick={handleSearch} disabled={loading || !selectedCourse}>
                                {loading ? <CSpinner size="sm" /> : <><Search size={18} className="me-1" /> Search</>}
                            </CButton>
                        </CCol>
                    </CRow>
                </CCardBody>
            </CCard>
            <CCard className="mb-4">
                <CCardHeader>
                    <strong>Attendance Statistics</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow className="mb-4">
                        <CCol md={4}>
                            <CFormSelect
                                label="Course for Stats"
                                value={chartCourse}
                                onChange={(e) => setChartCourse(e.target.value)}
                            >
                                <option value="">Select Course</option>
                                {courseOptions}
                            </CFormSelect>
                        </CCol>
                        <CCol md={4}>
                            <CFormInput
                                type="month"
                                label="Month"
                                value={chartMonth}
                                onChange={(e) => setChartMonth(e.target.value)}
                            />
                        </CCol>
                    </CRow>

                    <CRow className="justify-content-center">
                        <CCol md={6} lg={4}>
                            {loadingStats ? (
                                <div className="text-center p-5">
                                    <CSpinner />
                                </div>
                            ) : attendanceStats ? (
                                <div style={doughnutContainerStyle}>
                                    <CChart
                                        type="doughnut"
                                        customTooltips={false}
                                        height={DOUGHNUT_CANVAS_SIZE}
                                        width={DOUGHNUT_CANVAS_SIZE}
                                        style={{ height: `${DOUGHNUT_CANVAS_SIZE}px`, width: `${DOUGHNUT_CANVAS_SIZE}px` }}
                                        data={doughnutData ?? { labels: [], datasets: [] }}
                                        options={doughnutOptions}
                                    />
                                </div>
                            ) : (
                                <div className="text-center text-muted p-5">
                                    Select a course and month to view statistics
                                </div>
                            )}
                        </CCol>
                    </CRow>
                </CCardBody>
            </CCard>
            {attendanceRecords.length > 0 && (
                <CCard>
                    <CCardHeader className="d-flex justify-content-between align-items-center">
                        <strong>Student Status</strong>
                        <div>
                            <CButton color="success" className="me-2 text-white" onClick={handleSave} disabled={saving}>
                                <Save size={18} className="me-1" /> Save
                            </CButton>
                            <CButton color="info" className="text-white" onClick={handleExport}>
                                <Download size={18} className="me-1" /> Export
                            </CButton>
                        </div>
                    </CCardHeader>
                    <CCardBody>
                        <CTable hover responsive>
                            <CTableHead>
                                <CTableRow>
                                    <CTableHeaderCell>ID</CTableHeaderCell>
                                    <CTableHeaderCell>Student</CTableHeaderCell>
                                    <CTableHeaderCell>Attendance Status</CTableHeaderCell>
                                    <CTableHeaderCell>Note</CTableHeaderCell>
                                </CTableRow>
                            </CTableHead>
                            <CTableBody>
                                {attendanceRecords.map((record) => (
                                    <CTableRow key={record.id}>
                                        <CTableDataCell>{record.student.indexNumber}</CTableDataCell>
                                        <CTableDataCell>
                                            <div className="d-flex align-items-center">
                                                <CAvatar src={record.student.avatarUrl} size="md" className="me-2" />
                                                <div>
                                                    <div>{record.student.firstName} {record.student.lastName}</div>
                                                    <small className="text-muted">ID: {record.student.id}</small>
                                                </div>
                                            </div>
                                        </CTableDataCell>
                                        <CTableDataCell>
                                            <div className="d-flex gap-2">
                                                {attendanceStatusButtons.map((btn) => (
                                                    <CButton
                                                        key={btn.status}
                                                        size="sm"
                                                        color={btn.color}
                                                        variant={record.status === btn.status ? undefined : 'outline'}
                                                        onClick={() => handleStatusChange(record.id, btn.status)}
                                                    >
                                                        {btn.label}
                                                    </CButton>
                                                ))}
                                            </div>
                                        </CTableDataCell>
                                        <CTableDataCell>
                                            <CFormInput
                                                size="sm"
                                                placeholder="Optional note..."
                                                value={record.note || ''}
                                                onChange={(e) => handleNoteChange(record.id, e.target.value)}
                                            />
                                        </CTableDataCell>
                                    </CTableRow>
                                ))}
                            </CTableBody>
                        </CTable>
                    </CCardBody>
                </CCard>
            )}
        </div>
    );
}
