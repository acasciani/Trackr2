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
	public partial class Transaction : System.Runtime.Serialization.ISerializable
	{
		private long _transactionID;
		public virtual long TransactionID
		{
			get
			{
				return this._transactionID;
			}
			set
			{
				this._transactionID = value;
			}
		}
		
		private decimal _amount;
		public virtual decimal Amount
		{
			get
			{
				return this._amount;
			}
			set
			{
				this._amount = value;
			}
		}
		
		private int _transactionTypeID;
		public virtual int TransactionTypeID
		{
			get
			{
				return this._transactionTypeID;
			}
			set
			{
				this._transactionTypeID = value;
			}
		}
		
		private string _publicNotes;
		public virtual string PublicNotes
		{
			get
			{
				return this._publicNotes;
			}
			set
			{
				this._publicNotes = value;
			}
		}
		
		private string _privateNotes;
		public virtual string PrivateNotes
		{
			get
			{
				return this._privateNotes;
			}
			set
			{
				this._privateNotes = value;
			}
		}
		
		private DateTime _date;
		public virtual DateTime Date
		{
			get
			{
				return this._date;
			}
			set
			{
				this._date = value;
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
		
		private bool _active;
		public virtual bool Active
		{
			get
			{
				return this._active;
			}
			set
			{
				this._active = value;
			}
		}
		
		private DateTime _createDt;
		public virtual DateTime CreateDt
		{
			get
			{
				return this._createDt;
			}
			set
			{
				this._createDt = value;
			}
		}
		
		private int _createUserID;
		public virtual int CreateUserID
		{
			get
			{
				return this._createUserID;
			}
			set
			{
				this._createUserID = value;
			}
		}
		
		private DateTime? _modifyDt;
		public virtual DateTime? ModifyDt
		{
			get
			{
				return this._modifyDt;
			}
			set
			{
				this._modifyDt = value;
			}
		}
		
		private int? _modifyUserID;
		public virtual int? ModifyUserID
		{
			get
			{
				return this._modifyUserID;
			}
			set
			{
				this._modifyUserID = value;
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
		
		private TransactionType _transactionType;
		public virtual TransactionType TransactionType
		{
			get
			{
				return this._transactionType;
			}
			set
			{
				this._transactionType = value;
			}
		}
		
		private WebUserInfo _webUserInfo;
		public virtual WebUserInfo CreateUser
		{
			get
			{
				return this._webUserInfo;
			}
			set
			{
				this._webUserInfo = value;
			}
		}
		
		private WebUserInfo _webUserInfo1;
		public virtual WebUserInfo ModifyUser
		{
			get
			{
				return this._webUserInfo1;
			}
			set
			{
				this._webUserInfo1 = value;
			}
		}
		
		#region ISerializable Implementation
		
		public Transaction()
		{
		}
		
		protected Transaction(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			this.TransactionID = info.GetInt64("TransactionID");
			this.Amount = info.GetDecimal("Amount");
			this.TransactionTypeID = info.GetInt32("TransactionTypeID");
			this.PublicNotes = info.GetString("PublicNotes");
			this.PrivateNotes = info.GetString("PrivateNotes");
			this.Date = (DateTime)info.GetValue("Date", typeof(DateTime));
			this.PlayerID = info.GetInt32("PlayerID");
			this.Active = info.GetBoolean("Active");
			this.CreateDt = (DateTime)info.GetValue("CreateDt", typeof(DateTime));
			this.CreateUserID = info.GetInt32("CreateUserID");
			this.ModifyDt = (DateTime?)info.GetValue("ModifyDt", typeof(DateTime?));
			this.ModifyUserID = (int?)info.GetValue("ModifyUserID", typeof(int?));
			CustomizeDeserializationProcess(info, context);
		}
		
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("TransactionID", this.TransactionID, typeof(long));
			info.AddValue("Amount", this.Amount, typeof(decimal));
			info.AddValue("TransactionTypeID", this.TransactionTypeID, typeof(int));
			info.AddValue("PublicNotes", this.PublicNotes, typeof(string));
			info.AddValue("PrivateNotes", this.PrivateNotes, typeof(string));
			info.AddValue("Date", this.Date, typeof(DateTime));
			info.AddValue("PlayerID", this.PlayerID, typeof(int));
			info.AddValue("Active", this.Active, typeof(bool));
			info.AddValue("CreateDt", this.CreateDt, typeof(DateTime));
			info.AddValue("CreateUserID", this.CreateUserID, typeof(int));
			info.AddValue("ModifyDt", this.ModifyDt, typeof(DateTime?));
			info.AddValue("ModifyUserID", this.ModifyUserID, typeof(int?));
			CustomizeSerializationProcess(info, context);
		}
		
		partial void CustomizeSerializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		partial void CustomizeDeserializationProcess(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context);
		#endregion
	}
}
#pragma warning restore 1591
