namespace Faculty.Application.Interfaces
{
    public interface ITeacherService
    {
        Task<int?> GetTeacherIDByUserID(Guid userID);
    }
}
