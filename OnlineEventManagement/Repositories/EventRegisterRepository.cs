
using System.Data.SqlClient;
using System.Data;

namespace OnlineEventManagement.Repositories
{
    public class EventRegisterRepository : IEventRegisterRepository
    {
        private readonly IConfiguration configuration;

        public EventRegisterRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Add Event register
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="createdBy"></param>
        /// <returns></returns>
        public async Task<Guid?> AddEventRegisterAsync(Guid eventId, Guid createdBy)
        {
            Guid? newRegisterId = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_AddEventRegister", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@EventId", SqlDbType.UniqueIdentifier)).Value = eventId;
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.UniqueIdentifier)).Value = createdBy;

                        // Output parameter for new register ID
                        SqlParameter newRegisterIdParam = new SqlParameter("@NewRegisterId", SqlDbType.UniqueIdentifier)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(newRegisterIdParam);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();

                        // Retrieve the output value (if any)
                        if (newRegisterIdParam.Value != DBNull.Value)
                        {
                            newRegisterId = (Guid?)newRegisterIdParam.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return newRegisterId;
        }
    }
}
