#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the ClassGenerator.ttinclude code generation file.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using Telerik.OpenAccess.Data.Common;
using Telerik.OpenAccess.Metadata.Fluent;
using Telerik.OpenAccess.Metadata.Fluent.Advanced;
using TrackrModels;

namespace TrackrModels	
{
	[System.Serializable()]
	public partial class Guardian : System.Runtime.Serialization.ISerializable
	{
		private int _guardianID;
		public virtual int GuardianID
		{
			get
			{
				return this._guardianID;
			}
			set
			{
				this._guardianID = value;
			}
		}
		
		private int _personID;
		public virtual int PersonID
		{
			get
			{
				return this._personID;
			}
			set
			{
				this._personID = value;
			}
		}
		
		private int _playerID;
		public virtual int PlayerID
		{
			get
			{
				return this._playerID;
			}
			set
			{
				this._playerID = value;
			}
		}
		
		private byte _sortOrder;
		public virtual byte SortOrder
		{
			get
			{
				return this._sortOrder;
			}
			set
			{
				this._sortOrder = value;
			}
		}
		
		private Player _player;
		public virtual Player Player
		{
			get
			{
				return this._player;
			}
			set
			{
				this._player = value;
			}
		}
		
		private Person _person;
		public virtual Person Person
		{
			get
			{
				return this._person;
			}
			set
			{
				this._person = value;
			}
		}
		
		#region ISerializable Implementation
		
		public Guardian()
		{
		}
		
		protected Guardian(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.GuardianID = info.GetInt32("GuardianID");
			this.PersonID = info.GetInt32("PersonID");
			this.PlayerID = info.GetInt32("PlayerID");
			this.SortOrder = info.GetByte("SortOrder");
			CustomizeDeserializationProcess(info, context);
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("GuardianID", this.GuardianID, typeof(int));
			info.AddValue("PersonID", this.PersonID, typeof(int));
			info.AddValue("PlayerID", this.PlayerID, typeof(int));
			info.AddValue("SortOrder", this.SortOrder, typeof(byte));
			CustomizeSerializationProcess(info, context);
		}
		
		partial void CustomizeSerializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		partial void CustomizeDeserializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		#endregion
	}
}
#pragma warning restore 1591