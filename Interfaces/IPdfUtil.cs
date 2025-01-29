using FillableFormWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FillableFormWebApp.Interfaces
{
    public interface IPdfUtil
    {
        /// <summary>
        /// Find the corresponding fillable pdf and fills the avalible fields
        /// </summary>
        /// <param name="form"></param>
        /// <returns>The MemoryStream containing the pdf file</returns>
        /// <exception cref="Exception"></exception>
        MemoryStream FillForm(Form form);
    }
}