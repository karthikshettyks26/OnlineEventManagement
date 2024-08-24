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

        /// <summary>
        /// Get all the events - apply filter and sorting.
        /// </summary>
        /// <param name="filterOn"></param>
        /// <param name="filterQuery"></param>
        /// <param name="sortBy"></param>
        /// <param name="isAscending"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Event>> GetAllEventsAsync(string? filterOn, string? filterQuery, string? sortBy, bool? isAscending = true)
        {
            IEnumerable<Event> eventsList = null;
            var events = new List<Event>();

            try
            {
                #region Query
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_GetAllEvents", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var @event = new Event
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                    CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                    EventDate = reader.GetDateTime(reader.GetOrdinal("EventDate")),
                                    MaxParticipants = reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
                                };

                                events.Add(@event);
                            }
                        }
                    }
                }
                #endregion

                //Add to IEnumerable
                eventsList = events;

                #region  Filter
                if(eventsList != null && eventsList.Any() && !string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
                {
                    if (filterOn.Equals("Title", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.Title.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("Description", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.Description.Contains(filterQuery));
                    }
                    else if (filterOn.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.CreatedBy.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.CreatedOn.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("EventDate", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.EventDate.Equals(filterQuery));
                    }
                    else if (filterOn.Equals("MaxParticipants", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = eventsList.Where(x => x.MaxParticipants.Equals(filterQuery));
                    }
                }
                #endregion

                #region Sorting
                if (eventsList != null && eventsList.Any() && !string.IsNullOrWhiteSpace(sortBy))
                {
                    if (sortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.Title) :eventsList.OrderByDescending(x => x.Title);
                    }
                    else if (sortBy.Equals("Description", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.Description) : eventsList.OrderByDescending(x => x.Description);
                    }
                    else if (sortBy.Equals("CreatedBy", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.CreatedBy) : eventsList.OrderByDescending(x => x.CreatedBy);
                    }
                    else if (sortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.CreatedOn) : eventsList.OrderByDescending(x => x.CreatedOn);
                    }
                    else if (sortBy.Equals("EventDate", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.EventDate) : eventsList.OrderByDescending(x => x.EventDate);
                    }
                    else if (sortBy.Equals("MaxParticipants", StringComparison.OrdinalIgnoreCase))
                    {
                        eventsList = isAscending == true ? eventsList.OrderBy(x => x.MaxParticipants) : eventsList.OrderByDescending(x => x.MaxParticipants);
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return eventsList;
        }

        /// <summary>
        /// Get Event By id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<Event> GetEventByIdAsync(Guid eventId)
        {
            Event @event = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_GetEventById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameter
                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier)).Value = eventId;

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                @event = new Event
                                {
                                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    CreatedBy = reader.GetGuid(reader.GetOrdinal("CreatedBy")),
                                    CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                    EventDate = reader.GetDateTime(reader.GetOrdinal("EventDate")),
                                    MaxParticipants = reader.GetInt32(reader.GetOrdinal("MaxParticipants"))
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return @event;
        }

        /// <summary>
        /// Update event
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task<bool> UpdateEventAsync(Event @event, Guid Id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_UpdateEvent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier)).Value = @event.Id;
                        command.Parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar, 100)).Value = @event.Title;
                        command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 100)).Value = @event.Description ?? (object)DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@EventDate", SqlDbType.DateTime)).Value = @event.EventDate;
                        command.Parameters.Add(new SqlParameter("@MaxParticipants", SqlDbType.Int)).Value = @event.MaxParticipants;

                        // Execute the command
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // If one or more rows were affected, return true (indicating success)
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Delete event by Id
        /// </summary>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_DeleteEvent", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add the parameter
                        command.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier)).Value = eventId;

                        // Execute the command
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // If one or more rows were affected, return true (indicating success)
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

    }
}
