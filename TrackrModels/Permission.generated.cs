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
	public partial class Permission
	{
		private int _permissionID;
		public virtual int PermissionID
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
		
		private string _permissionName;
		public virtual string PermissionName
		{
			get
			{
				return this._permissionName;
			}
			set
			{
				this._permissionName = value;
			}
		}
		
		private IList<Role> _roles = new List<Role>();
		public virtual IList<Role> Roles
		{
			get
			{
				return this._roles;
			}
		}
		
		private IList<ScopeAssignment> _scopeAssignments = new List<ScopeAssignment>();
		public virtual IList<ScopeAssignment> ScopeAssignments
		{
			get
			{
				return this._scopeAssignments;
			}
		}
		
		private IList<LinkPermission> _linkPermissions = new List<LinkPermission>();
		public virtual IList<LinkPermission> LinkPermissions
		{
			get
			{
				return this._linkPermissions;
			}
		}
		
	}
}
#pragma warning restore 1591
