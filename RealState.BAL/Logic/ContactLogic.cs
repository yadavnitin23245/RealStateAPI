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

        private readonly IRepository<Contact> _ContactRepository;
        #endregion

        #region CTOR's
        public ContactLogic(

              IRepository<ContactDTO> ContactDTORepository, IRepository<Contact> ContactRepository, IRepository<ContactStatDTO> ContactStatDTORepository

            )
        {

            _ContactDTORepository = ContactDTORepository;
            _ContactRepository = ContactRepository;
            _ContactStatDTORepository = ContactStatDTORepository;
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
