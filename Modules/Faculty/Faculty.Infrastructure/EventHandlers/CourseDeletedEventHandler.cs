using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Application.Events;

namespace Faculty.Application.EventHandlers
{
    public class CourseDeletedEventHandler : INotificationHandler<CourseDeletedEvent>
    {
        public Task Handle(CourseDeletedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Faculty received CourseDeletedEvent: {notification.CourseId}");
            return Task.CompletedTask;
        }
    }

}
