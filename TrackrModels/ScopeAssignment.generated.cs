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
	public partial class ScopeAssignment
	{
		private int _scopeAssignmentID;
		public virtual int ScopeAssignmentID
		{
			get
			{
				return this._scopeAssignmentID;
			}
			set
			{
				this._scopeAssignmentID = value;
			}
		}
		
		private int _userID;
		public virtual int UserID
		{
			get
			{
				return this._userID;
			}
			set
			{
				this._userID = value;
			}
		}
		
		private int? _roleID;
		public virtual int? RoleID
		{
			get
			{
				return this._roleID;
			}
			set
			{
				this._roleID = value;
			}
		}
		
		private int? _permissionID;
		public virtual int? PermissionID
		{
			get
			{
				return this._permissionID;
			}
			set
			{
				this._permissionID = value;
			}
		}
		
		private int _scopeID;
		public virtual int ScopeID
		{
			get
			{
				return this._scopeID;
			}
			set
			{
				this._scopeID = value;
			}
		}
		
		private int _resourceID;
		public virtual int ResourceID
		{
			get
			{
				return this._resourceID;
			}
			set
			{
				this._resourceID = value;
			}
		}
		
		private bool _isDeny;
		public virtual bool IsDeny
		{
			get
			{
				return this._isDeny;
			}
			set
			{
				this._isDeny = value;
			}
		}
		
		private bool _isExplicit;
		public virtual bool IsExplicit
		{
			get
			{
				return this._isExplicit;
			}
			set
			{
				this._isExplicit = value;
			}
		}
		
		private Permission _permission;
		public virtual Permission Permission
		{
			get
			{
				return this._permission;
			}
			set
			{
				this._permission = value;
			}
		}
		
		private Role _role;
		public virtual Role Role
		{
			get
			{
				return this._role;
			}
			set
			{
				this._role = value;
			}
		}
		
		private WebUser _webUser;
		public virtual WebUser WebUser
		{
			get
			{
				return this._webUser;
			}
			set
			{
				this._webUser = value;
			}
		}
		
		private Scope _scope;
		public virtual Scope Scope
		{
			get
			{
				return this._scope;
			}
			set
			{
				this._scope = value;
			}
		}
		
	}
}
#pragma warning restore 1591
