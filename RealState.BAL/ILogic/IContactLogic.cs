using RealState.BAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealState.BAL.ILogic
{
    public interface IContactLogic
    {

        #region Contact
        Task<string> AddContact(ContactDTO obj);

      //  List<ContactDTO> SaveContactDetail(ContactDTO values);
        #endregion
    }
}
