using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Application.Events;

namespace Faculty.Application.EventHandlers
{
    public class CourseUpdatedEventHandler : INotificationHandler<CourseUpdatedEvent>
    {
        public Task Handle(CourseUpdatedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Faculty received CourseUpdatedEvent: {notification.CourseId}");
            return Task.CompletedTask;
        }
    }
}
