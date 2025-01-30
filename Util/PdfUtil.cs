using FillableFormWebApp.Interfaces;
using FillableFormWebApp.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using static FillableFormWebApp.PdfForm;

namespace FillableFormWebApp.Util
{
    public class PdfUtil : IPdfUtil
    {
        private readonly List<FormClass> _pdfForm;
        public PdfUtil(IOptions<PdfForm> pdfForm) 
        { 
            _pdfForm = pdfForm.Value.Form; 
        }

        /// <summary>
        /// Find the corresponding fillable pdf and fills the avalible fields
        /// </summary>
        /// <param name="form"></param>
        /// <returns>The MemoryStream containing the pdf file</returns>
        /// <exception cref="Exception"></exception>
        public MemoryStream FillForm(Form form)
        {
            string fileName = "";
            switch (form.FormTypeId)
            {
                case 1:
                    fileName = "LAF-nosignature.pdf";
                    break;
                default:
                    break;
            }
            string ogFormPath = Directory.GetCurrentDirectory() + "/Pdf/" + fileName;
            try
            {
                PdfDocument pdf = PdfReader.Open(ogFormPath);
                var pdfFormFile = pdf.AcroForm;

                if (pdfFormFile.Elements.ContainsKey("/NeedAppearances"))
                {
                    pdfFormFile.Elements["/NeedAppearances"] = new PdfBoolean(true);
                }
                else
                {
                    pdfFormFile.Elements.Add("/NeedAppearances", new PdfBoolean(true));
                }

                switch (form.FormTypeId)
                {
                    case 1:
                        pdfFormFile = FillFormLeaveOfAbsence(pdfFormFile, form);
                        break;
                    default:
                        throw new Exception("FormType not found");
                }
                pdf.Flatten();

                MemoryStream stream = new();
                pdf.Save(stream, false);
                return stream;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Fills the fillable pdf 'Leave of Absence'
        /// </summary>
        /// <param name="pdfFormFile"></param>
        /// <param name="form"></param>
        /// <returns>The provided PdfAcroForm filled</returns>
        /// <exception cref="NullReferenceException"></exception>
        private PdfAcroForm FillFormLeaveOfAbsence(PdfAcroForm pdfFormFile, Form form)
        {
            PdfString pdfString = new PdfString();

            if (pdfFormFile.Fields["employeeName"] == null)
                throw new NullReferenceException("Field employeeName not found");
            pdfFormFile.Fields["employeeName"].Value = new PdfString(form.Employee.EmployeeName);

            if (pdfFormFile.Fields["date"] == null)
                throw new NullReferenceException("Field date not found");
            pdfFormFile.Fields["date"].Value = new PdfString(form.FormDate.Value.ToString("MM-dd-yyyy"));

            if (pdfFormFile.Fields["ssn"] == null)
                throw new NullReferenceException("Field ssn not found");
            pdfFormFile.Fields["ssn"].Value = new PdfString(form.Employee.Ssn.ToString());

            if (pdfFormFile.Fields["organization"] == null)
                throw new NullReferenceException("Field organization not found");
            pdfFormFile.Fields["organization"].Value = new PdfString("Fictional Org");

            if (pdfFormFile.Fields["department"] == null)
                throw new NullReferenceException("Field department not found");
            pdfFormFile.Fields["department"].Value = new PdfString(form.Employee.Department.DepartmentName);

            if (pdfFormFile.Fields["datesOfLeave"] == null)
                throw new NullReferenceException("Field datesOfLeave not found");
            pdfFormFile.Fields["datesOfLeave"].Value = new PdfString(form.Dates);

            string? typeOfLeave = null;
            if (form.TypeOfLeave.Split(" ").Length < 2)
            {
                typeOfLeave = form.TypeOfLeave.ToLower();
            }
            else 
            {
                typeOfLeave = form.TypeOfLeave.ToCamelCase(" ");
            }

            if (pdfFormFile.Fields[typeOfLeave + "Chk"] is not PdfCheckBoxField typeOfLeaveChk)
                throw new NullReferenceException("Type Of Leave not found");
            typeOfLeaveChk.Checked = true;

            var formFieldName = "Justification";
            pdfFormFile = FillMultipleValues(pdfFormFile, form, formFieldName);

            if (form.TypeOfLeave == "Other")
            {
                formFieldName = "Other";
                pdfFormFile = FillMultipleValues(pdfFormFile, form, formFieldName);
            }

            if (form.AdditionalRemarks != null)
            {
                formFieldName = "AdditionalRemarks";
                pdfFormFile = FillMultipleValues(pdfFormFile, form, formFieldName);
            }

            if (form.Decision == "Approved")
            {
                if (pdfFormFile.Fields["approvedChk"] is not PdfCheckBoxField approvedChk)
                    throw new NullReferenceException("Approved Check box not found");
                approvedChk.Checked = true;
            }
            else
            {
                if (pdfFormFile.Fields["disapprovedChk"] is not PdfCheckBoxField disapprovedChk)
                    throw new NullReferenceException("disapproved Check box not found");
                disapprovedChk.Checked = true;

                if (form.Reason != null)
                {
                    formFieldName = "Reason";
                    pdfFormFile = FillMultipleValues(pdfFormFile, form, formFieldName);
                }
            }

            return pdfFormFile;
        }

        /// <summary>
        /// Fills the given fillable pdf field even if it has multiple lines<br />
        /// The fields need to be named with a '0 + the line number'<br /><br />
        /// Example: purposeForLeave01, purposeForLeave02 and purposeForLeave03
        /// </summary>
        /// <param name="pdfFormFile"></param>
        /// <param name="form"></param>
        /// <param name="formFieldName"></param>
        /// <returns>The given PdfAcroForm with one or multiple lines filled</returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private PdfAcroForm FillMultipleValues(PdfAcroForm pdfFormFile, Form form, string formFieldName)
        {
            var pdfFieldName = GetPdfFieldName(formFieldName);
            var formClass = _pdfForm.Where(x => x.FormTypeId == form.FormTypeId).SingleOrDefault();
            if (formClass == null)
                throw new Exception("Pdf fields not found");

            var fields = formClass.Fields.Where(f => f.FieldName.Contains(pdfFieldName) && !f.FieldName.Contains("Chk")).ToList();

            if (form.GetType().GetProperty(formFieldName).GetValue(form) is not String formFieldValue)
                throw new Exception("Form field not found");            

            if (formFieldValue.Length <= fields[0].MaxLength)
            {
                if (pdfFormFile.Fields[pdfFieldName + "01"] == null)
                    throw new ArgumentException("Pdf field name not found. Make sure that the field name has a '01' at the end");

                if (form.GetType().GetProperty(formFieldName).GetValue(form) is not String fieldValue)
                    throw new NullReferenceException("The field doesn't exist");

                pdfFormFile.Fields[pdfFieldName + "01"].Value = new PdfString(fieldValue);
            }
            else
            {
                var fieldSplit = formFieldValue.Split(" ");
                string[]? fieldArray = SplitValueIntoFields(form, formFieldName, fields);

                for (int i = 0; i < fieldArray.Length; i++)
                {
                    if (pdfFormFile.Fields[$"{pdfFieldName}0{i + 1}"] == null)
                        throw new NullReferenceException("The field doesn't exist");

                    pdfFormFile.Fields[$"{pdfFieldName}0{i + 1}"].Value = new PdfString(fieldArray[i]);
                }                   
            }
            return pdfFormFile;
        }

        /// <summary>
        /// A switch to match the table's column name to the name of the fillable pdf field
        /// </summary>
        /// <param name="formField"></param>
        /// <returns>The fillable pdf field</returns>
        /// <exception cref="ArgumentException"></exception>
        private static string GetPdfFieldName(string formField)
        {
            switch (formField)
            {
                case "Justification":
                    return "purposeForLeave";
                case "Other":
                    return "other";
                case "AdditionalRemarks":
                    return "additionalRemarks";
                case "Reason":
                    return "reasonForDisapproval";
                default:
                    throw new ArgumentException();
            }
        }


        /// <summary>
        /// Gets the value of the given property and splits the string into a array that fits the maximum length of the given fillable pdf field <br />
        /// If the value's length is greater than the field MaxLength, it will break the loop at maximum allowed length
        /// </summary>
        /// <param name="form"></param>
        /// <param name="property"></param>
        /// <param name="fieldNameCount"></param>
        /// <returns>A array of strings that fits in the field of the fillable pdf</returns>
        /// <exception cref="NullReferenceException"></exception>
        private static string[] SplitValueIntoFields(Form form, string property, List<FormClass.Field> fieldName)
        {
            if (form.GetType().GetProperty(property).GetValue(form) is not String fieldValue)
                throw new NullReferenceException("The field doesn't exist");

            var fieldValueSplit = fieldValue.Split(" ");
            string[]? fieldValueArray = new string[fieldName.Count()];
            var fieldIndex = 0;

            for (int i = 0; i < fieldValueSplit.Length; i++)
            {
                if (fieldIndex >= fieldValueArray.Length)
                    break;

                if (fieldValueArray[fieldIndex] == null)
                {
                    fieldValueArray[fieldIndex] = fieldValueSplit[i];
                }
                else
                {
                    if ((fieldValueArray[fieldIndex] + " " + fieldValueSplit[i]).Length > fieldName[fieldIndex].MaxLength)
                    {
                        fieldIndex++;
                        i--;
                        continue;
                    }
                    fieldValueArray[fieldIndex] = fieldValueArray[fieldIndex] + " " + fieldValueSplit[i];
                }
            }
            return fieldValueArray;
        }
    }
}
