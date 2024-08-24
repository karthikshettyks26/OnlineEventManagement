using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using OnlineEventManagement.Models.Domain;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace OnlineEventManagement.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IConfiguration configuration;

        public EventRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        

        /// <summary>
        /// This method inserts Event record to Database.
        /// </summary>
        /// <param name="event"></param>
        public async Task<Guid> AddEventAsync(Event @event)
        {
            Guid newEventGuid = Guid.Empty;
            try
            {
                // Create and open the connection to SQL Server
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    // Create the SqlCommand and specify the stored procedure name
                    using (SqlCommand command = new SqlCommand("sp_AddEvent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 100)).Value = @event.Title;
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 100)).Value = @event.Description ?? (object)DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.UniqueIdentifier)).Value = @event.CreatedBy;
                        command.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime)).Value = DateTime.Now;
                        command.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.DateTime)).Value = @event.EventDate;
                        command.Parameters.Add(new SqlParameter("@MaxParticipants", SqlDbType.Int)).Value = @event.MaxParticipants;

                        //New Event Id.
                        SqlParameter newEventIdParam = new SqlParameter("@NewEventId", SqlDbType.UniqueIdentifier)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(newEventIdParam);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();
                        await connection.CloseAsync();

                        // Retrieve the output value (if any)
                        newEventGuid = (Guid)newEventIdParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return newEventGuid;
        }

        public Task<IEnumerable<Event>> GetAllEvents()
        {
            throw new NotImplementedException();
        }
    }
}
