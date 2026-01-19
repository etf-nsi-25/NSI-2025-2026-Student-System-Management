import React from 'react';
import { CButton, CBadge } from '@coreui/react';
import type { ExamDTO } from '../dto/CourseOverviewDTO';

interface Props {
    exams?: ExamDTO[]; 
    brandBlue: string;
    colors: { danger: string };
}

export const ExamsSection: React.FC<Props> = ({ exams = [], brandBlue, colors }) => {
    return (
        <div className="bg-white p-4 rounded-4 shadow-sm mb-4 border-0">
            <h3 className="mb-4 fw-bold" style={{ color: brandBlue }}>Exams</h3>
            
            {exams && exams.length > 0 ? (
                exams.map((exam) => (
                    <div key={exam.id} className="d-flex align-items-center mb-3 p-3 rounded-3 shadow-sm" style={{ backgroundColor: '#f0f7fa' }}>
                        
                       
                        <div className="rounded-3 d-flex align-items-center justify-content-center text-white fw-bold me-3" 
                             style={{ 
                                 width: '55px', 
                                 height: '45px', 
                                 backgroundColor: exam.status === 'FAILED' ? colors.danger : brandBlue, 
                                 fontSize: '1.2rem' 
                             }}>
                            {exam.status === 'FAILED' ? '!' : '✓'}
                        </div>

                       
                        <div className="flex-grow-1">
                            <h6 className="mb-0 fw-bold" style={{ fontSize: '1.1rem' }}>{exam.name || 'Unnamed Exam'}</h6>
                        </div>

                    
                        <div className="text-end">
                            <div className="small text-muted mb-1 fw-bold">• {exam.date || 'Date TBD'}</div>
                            
                            {exam.registerable && (
                                <CButton size="sm" className="text-white px-4 shadow-sm" style={{ backgroundColor: '#4b39ef', border: 'none', borderRadius: '6px' }}>
                                    Register
                                </CButton>
                            )}
                            
                            {exam.status === 'FAILED' && <CBadge color="danger" className="ms-2">Failed</CBadge>}
                            {exam.status === 'COMPLETED' && <CBadge color="success" className="ms-2">Passed</CBadge>}
                            {exam.status === 'UPCOMING' && <CBadge color="info" className="ms-2">Upcoming</CBadge>}
                        </div>
                    </div>
                ))
            ) : (
                <div className="p-3 text-center border rounded-3" style={{ borderStyle: 'dashed !important', color: '#6c757d' }}>
                    <p className="mb-0 italic">No exam information available for this course.</p>
                </div>
            )}
        </div>
    );
};