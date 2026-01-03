type Props = {
    semesters: string[];
    courses: { id: number; name: string }[];
    onLoadGrades: () => void;
};

export default function CourseSelector({
    semesters,
    courses,
    onLoadGrades
}: Props) {
    return (
        <div className="course-selector">
            <div>
                <label>Semester</label>
                <select>
                    {semesters.map(s => (
                        <option key={s}>{s}</option>
                    ))}
                </select>
            </div>

            <div>
                <label>Course</label>
                <select>
                    {courses.map(c => (
                        <option key={c.id}>{c.name}</option>
                    ))}
                </select>
            </div>

            <button className="primary" onClick={onLoadGrades}>
                Load grades
            </button>
            <button disabled>Calculate final grades</button>
        </div>
    );
}
