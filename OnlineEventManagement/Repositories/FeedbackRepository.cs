using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using OnlineEventManagement.Models.Domain;
using System.Data.SqlClient;
using System.Data;
using System.Xml.Linq;

namespace OnlineEventManagement.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly IConfiguration configuration;

        public FeedbackRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Add Feedback.
        /// </summary>
        /// <param name="feedback"></param>
        /// <returns></returns>
        public async Task<Guid?> AddFeedbackAsync(Feedback feedback)
        {
            Guid? newFeedbackId = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("sp_AddFeedback", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.Add(new SqlParameter("@Rating", SqlDbType.Int)).Value = feedback.Rating;
                        command.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar, 500)).Value = (object)feedback.comment ?? DBNull.Value;
                        command.Parameters.Add(new SqlParameter("@EventId", SqlDbType.UniqueIdentifier)).Value = feedback.EventId;
                        command.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.UniqueIdentifier)).Value = feedback.CreatedBy;

                        // Output parameter for new feedback ID
                        SqlParameter newFeedbackIdParam = new SqlParameter("@NewFeedbackId", SqlDbType.UniqueIdentifier)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(newFeedbackIdParam);

                        // Execute the command
                        await command.ExecuteNonQueryAsync();

                        // Retrieve the output value (if any)
                        if (newFeedbackIdParam.Value != DBNull.Value)
                        {
                            newFeedbackId = (Guid?)newFeedbackIdParam.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return newFeedbackId;
        }
    }
}
