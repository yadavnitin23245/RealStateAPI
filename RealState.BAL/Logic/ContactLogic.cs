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
    public class ContactLogic: IContactLogic
    {

        #region Private properties
        private readonly IRepository<ContactDTO> _ContactDTORepository;

        private readonly IRepository<ContactStatDTO> _ContactStatDTORepository;

        private readonly IRepository<PaymentFrequencyDTO> _PaymentFrequencyDTORepository;
        private readonly IRepository<AmortizationfrequencyDTO> _AmortizationfrequencyDTORepository;

        private readonly IRepository<Contact> _ContactRepository;
        private readonly IRepository<canadacitiesDTO> _canadacitiesDTORepository;
        #endregion

        #region CTOR's
        public ContactLogic(

              IRepository<ContactDTO> ContactDTORepository, IRepository<Contact> ContactRepository, IRepository<ContactStatDTO> ContactStatDTORepository,
               IRepository<PaymentFrequencyDTO> PaymentFrequencyDTORepository, IRepository<AmortizationfrequencyDTO> AmortizationfrequencyDTORepository,
               IRepository<canadacitiesDTO> canadacitiesDTORepository

            )
        {

            _ContactDTORepository = ContactDTORepository;
            _ContactRepository = ContactRepository;
            _ContactStatDTORepository = ContactStatDTORepository;
            _PaymentFrequencyDTORepository = PaymentFrequencyDTORepository;
            _AmortizationfrequencyDTORepository = AmortizationfrequencyDTORepository;
            _canadacitiesDTORepository = canadacitiesDTORepository;
        }

        #endregion


        public async Task<string> AddContact(ContactDTO obj)
        {
            try
            {
                
                Contact ContactObj = MapDTOToModel(obj);
                await _ContactRepository.InsertAsync(ContactObj);
                await _ContactRepository.SaveChangesAsync();
                return ContactObj.Id.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ContactStatDTO> GetContactStats(string op)
        {
            try
            {
                string procName = SPROC_Names.UspGetContactStats.ToString();
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };
                var resultData = _ContactStatDTORepository.ExecuteWithJsonResult(procName, "ContactStatDTO", ParamsArray);

                return resultData != null ? resultData.ToList() : new List<ContactStatDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<AmortizationfrequencyDTO> GetAmortizationfrequency(string op)
        {
            try
            {
                string procName = SPROC_Names.UspGetAmortizationfrequency.ToString();
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };
                var resultData = _AmortizationfrequencyDTORepository.ExecuteWithJsonResult(procName, "AmortizationfrequencyDTO", ParamsArray);

                return resultData != null ? resultData.ToList() : new List<AmortizationfrequencyDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<canadacitiesDTO> Getcanadacities(string op)
        {
            try
            {
                string procName = SPROC_Names.UspGetcanadacities.ToString();
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };
                var resultData = _canadacitiesDTORepository.ExecuteWithJsonResult(procName, "canadacitiesDTO", ParamsArray);

                return resultData != null ? resultData.ToList() : new List<canadacitiesDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PaymentFrequencyDTO> GetPaymentFrequrency(string op)
        {
            try
            {
                string procName = SPROC_Names.UspGetPaymentFrequrency.ToString();
                var ParamsArray = new SqlParameter[1];
                ParamsArray[0] = new SqlParameter() { ParameterName = "@OpCode", Value = "", DbType = System.Data.DbType.String };
                var resultData = _PaymentFrequencyDTORepository.ExecuteWithJsonResult(procName, "PaymentFrequencyDTO", ParamsArray);

                return resultData != null ? resultData.ToList() : new List<PaymentFrequencyDTO>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Contact MapDTOToModel(ContactDTO obj)
        {
            return new Contact()
            {
                Id = obj.Id,
                Name = obj.Name,
                PhoneNumber = obj.PhoneNumber,
                Subject=obj.Subject,
                Message=obj.Message,
                EmailSend= false,
                Email = obj.Email,
                IsActive = obj.IsActive,
                CreatedDate = obj.CreatedDate,
                lastName = obj.lastName,
                TimeToContact = obj.TimeToContact,
            };
        }
    }
}
