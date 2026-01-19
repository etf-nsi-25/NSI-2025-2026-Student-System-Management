import React from 'react';
import { CButton } from '@coreui/react';
import type { AssignmentDTO } from '../dto/CourseOverviewDTO';

interface Props {
    assignments?: AssignmentDTO[];
    brandBlue: string;
}

export const AssignmentsSection: React.FC<Props> = ({ assignments = [], brandBlue }) => {
    return (
        <div className="bg-white p-4 rounded-4 shadow-sm border-0">
            <h3 className="mb-4 fw-bold" style={{ color: brandBlue }}>Assignments</h3>
            
            {assignments && assignments.length > 0 ? (
                assignments.map((task) => (
                    <div key={task.id} className="d-flex align-items-center mb-3 p-3 rounded-3 shadow-sm" style={{ backgroundColor: '#f0f7fa' }}>
                        
                        <div className="text-white rounded-3 fw-bold me-4 d-flex align-items-center justify-content-center" 
                             style={{ minWidth: '70px', height: '45px', backgroundColor: brandBlue }}>
                            {task.points ? task.points : 'Max'}
                        </div>

                        <div className="flex-grow-1">
                            <h6 className="mb-0 fw-bold" style={{ fontSize: '1.1rem' }}>
                                {task.name || 'Untitled Assignment'}
                            </h6>
                            <small className="text-muted fw-semibold">
                                {task.desc || 'No description provided.'}
                            </small>
                        </div>

                   
                        <div className="text-end small">
                            <div className="text-muted mb-1 fw-bold">
                                • {task.status === 'todo' ? `Due: ${task.dueDate || 'No date'}` : 'Marked'}
                            </div>
                            
                            {task.status === 'todo' && (
                                <CButton size="sm" className="text-white px-4 shadow-sm" style={{ backgroundColor: '#4b39ef', border: 'none', borderRadius: '6px' }}>
                                    Submit
                                </CButton>
                            )}
                            
                            {task.status === 'marked' && task.points && (
                                <div className="fw-bold h6 mb-0" style={{ color: brandBlue }}>
                                    Score: {task.points}
                                </div>
                            )}
                        </div>
                    </div>
                ))
            ) : (
                <div className="p-4 text-center border rounded-3" style={{ borderStyle: 'dashed !important', color: '#6c757d', backgroundColor: '#fafafa' }}>
                    <p className="mb-0 italic">No assignments assigned for this course yet.</p>
                </div>
            )}
        </div>
    );
};