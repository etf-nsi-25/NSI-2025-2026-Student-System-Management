using Analytics.Application.Interfaces;
using Analytics.Core.Entities;
using Analytics.Core.Constants;
using Faculty.Core.Interfaces;
using System.Text.Json.Nodes;

namespace Analytics.Application.Calculators;

public class GradeDistributionCalculator : IStatsCalculator
{
  public string MetricCode => MetricKey.GradeDistrib;
  public Scope Scope => Scope.Course;

  private readonly IAttendanceRepository _attendanceRepository;

  public GradeDistributionCalculator(IAttendanceRepository attendanceRepository)
  {
    _attendanceRepository = attendanceRepository;
  }

  public async Task<JsonObject> CalculateAsync(Guid courseId)
  {
    var enrollments = await _attendanceRepository.GetEnrolledStudentsAsync(courseId);

    var counts = new Dictionary<string, int>();

    foreach (var e in enrollments)
    {
      if (e.Grade == null) continue;
      var key = e.Grade.Value.ToString();
      if (counts.ContainsKey(key)) counts[key]++;
      else counts[key] = 1;
    }

    var obj = new JsonObject();
    foreach (var kv in counts.OrderBy(k => k.Key))
    {
      obj[kv.Key] = kv.Value;
    }

    return obj;
  }
}
