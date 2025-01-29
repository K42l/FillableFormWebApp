namespace FillableFormWebApp
{
    public class PdfForm
    {
        public List<FormClass> Form { get; set; }
        public class FormClass()
        {
            public int FormTypeId { get; set; }
            public string Name { get; set; }
            public Field[] Fields { get; set; }
            public class Field()
            {
                public string FieldName { get; set; }
                public int MaxLength { get; set; }
            }
        }
    }
}
