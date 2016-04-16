using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackrModels
{
    public partial class EmailAddress : IEditable
    {
        public Guid EditToken { get; set; }
        public bool WasModified { get; set; }

        partial void CustomizeSerializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("EditToken", this.EditToken, typeof(Guid));
            info.AddValue("WasModified", this.WasModified, typeof(bool));
        }

        partial void CustomizeDeserializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            this.EditToken = (Guid)info.GetValue("EditToken", typeof(Guid));
            this.WasModified = info.GetBoolean("WasModified");
        }
    }
}
