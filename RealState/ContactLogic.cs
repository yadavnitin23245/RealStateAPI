using Microsoft.Extensions.Options;
using RealState.BAL.DTO;
using RealState.BAL.ILogic;
using RealState.Data.Models;
using RealState.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealState.Common.ENUM.Common;

namespace RealState.BAL.Logic
{
    public class ContactLogic : IContactLogic
    {
        #region Private properties  
        // Logger instance for logging errors and information
        private readonly ILogger<ContactLogic> _logger;

        // Repositories for accessing data from the database
        private readonly IRepository<ContactDTO> _ContactDTORepository;
        private readonly IRepository<ContactStatDTO> _ContactStatDTORepository;
        private readonly IRepository<PaymentFrequencyDTO> _PaymentFrequencyDTORepository;
        private readonly IRepository<AmortizationfrequencyDTO> _AmortizationfrequencyDTORepository;
        private readonly IRepository<Contact> _ContactRepository;
        private readonly IRepository<canadacitiesDTO> _canadacitiesDTORepository;
        #endregion

        #region CTOR's  
        // Constructor to initialize dependencies via Dependency Injection
        public ContactLogic(
              ILogger<ContactLogic> logger,
              IRepository<ContactDTO> ContactDTORepository, IRepository<Contact> ContactRepository, IRepository<ContactStatDTO> ContactStatDTORepository,
              IRepository<PaymentFrequencyDTO> PaymentFrequencyDTORepository, IRepository<AmortizationfrequencyDTO> AmortizationfrequencyDTORepository,
              IRepository<canadacitiesDTO> canadacitiesDTORepository
            )
        {
            _logger = logger;
            _ContactDTORepository = ContactDTORepository;
            _ContactRepository = ContactRepository;
            _ContactStatDTORepository = ContactStatDTORepository;
            _PaymentFrequencyDTORepository = PaymentFrequencyDTORepository;
            _AmortizationfrequencyDTORepository = AmortizationfrequencyDTORepository;
            _canadacitiesDTORepository = canadacitiesDTORepository;
        }
        #endregion

        // Adds a new contact to the database
        public async Task<string> AddContact(ContactDTO obj)
        {
            try
            {
                // Map DTO to the Contact model
                Contact ContactObj = MapDTOToModel(obj);

                // Insert the contact into the repository
                await _ContactRepository.InsertAsync(ContactObj);

                // Save changes to the database
                await _ContactRepository.SaveChangesAsync();

                // Return the ID of the newly added contact
                return ContactObj.Id.ToString();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Error occurred in AddContact method.");
                throw;
            }
        }

        // Retrieves contact statistics from the database
        public List<ContactStatDTO> GetContactStats(string op)
        {
            try
            {
                // Stored procedure name
                string procName = SPROC_Names.UspGetContactStats.ToString();

                // Parameters for the stored procedure
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };

                // Execute the stored procedure and parse the result
                var resultData = _ContactStatDTORepository.ExecuteWithJsonResult(procName, "ContactStatDTO", ParamsArray);

                // Return the result as a list
                return resultData != null ? resultData.ToList() : new List<ContactStatDTO>();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Error occurred in GetContactStats method.");
                throw;
            }
        }

        // Retrieves amortization frequency data from the database
        public List<AmortizationfrequencyDTO> GetAmortizationfrequency(string op)
        {
            try
            {
                // Stored procedure name
                string procName = SPROC_Names.UspGetAmortizationfrequency.ToString();

                // Parameters for the stored procedure
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };

                // Execute the stored procedure and parse the result
                var resultData = _AmortizationfrequencyDTORepository.ExecuteWithJsonResult(procName, "AmortizationfrequencyDTO", ParamsArray);

                // Return the result as a list
                return resultData != null ? resultData.ToList() : new List<AmortizationfrequencyDTO>();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Error occurred in GetAmortizationfrequency method.");
                throw;
            }
        }

        // Retrieves a list of Canadian cities from the database
        public List<canadacitiesDTO> Getcanadacities(string op)
        {
            try
            {
                // Stored procedure name
                string procName = SPROC_Names.UspGetcanadacities.ToString();

                // Parameters for the stored procedure
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };

                // Execute the stored procedure and parse the result
                var resultData = _canadacitiesDTORepository.ExecuteWithJsonResult(procName, "canadacitiesDTO", ParamsArray);

                // Return the result as a list
                return resultData != null ? resultData.ToList() : new List<canadacitiesDTO>();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Error occurred in Getcanadacities method.");
                throw;
            }
        }

        // Retrieves payment frequency data from the database
        public List<PaymentFrequencyDTO> GetPaymentFrequrency(string op)
        {
            try
            {
                // Stored procedure name
                string procName = SPROC_Names.UspGetPaymentFrequrency.ToString();

                // Parameters for the stored procedure
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };

                // Execute the stored procedure and parse the result
                var resultData = _PaymentFrequencyDTORepository.ExecuteWithJsonResult(procName, "PaymentFrequencyDTO", ParamsArray);

                // Return the result as a list
                return resultData != null ? resultData.ToList() : new List<PaymentFrequencyDTO>();
            }
            catch (Exception ex)
            {
                // Log the error and rethrow the exception
                _logger.LogError(ex, "Error occurred in GetPaymentFrequrency method.");
                throw;
            }
        }

        // Maps a ContactDTO object to a Contact model
        private Contact MapDTOToModel(ContactDTO obj)
        {
            return new Contact()
            {
                Id = obj.Id,
                Name = obj.Name,
                PhoneNumber = obj.PhoneNumber,
                Subject = obj.Subject,
                Message = obj.Message,
                EmailSend = false,
                Email = obj.Email,
                IsActive = obj.IsActive,
                CreatedDate = obj.CreatedDate,
                lastName = obj.lastName,
                TimeToContact = obj.TimeToContact,
            };
        }
    }
}
