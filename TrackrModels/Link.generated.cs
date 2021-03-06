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
	public partial class Link
	{
		private int _linkID;
		public virtual int LinkID
		{
			get
			{
				return this._linkID;
			}
			set
			{
				this._linkID = value;
			}
		}
		
		private string _linkName;
		public virtual string LinkName
		{
			get
			{
				return this._linkName;
			}
			set
			{
				this._linkName = value;
			}
		}
		
		private string _linkURL;
		public virtual string LinkURL
		{
			get
			{
				return this._linkURL;
			}
			set
			{
				this._linkURL = value;
			}
		}
		
		private string _linkTitle;
		public virtual string LinkTitle
		{
			get
			{
				return this._linkTitle;
			}
			set
			{
				this._linkTitle = value;
			}
		}
		
		private int? _linkGroupID;
		public virtual int? LinkGroupID
		{
			get
			{
				return this._linkGroupID;
			}
			set
			{
				this._linkGroupID = value;
			}
		}
		
		private int _sortOrder;
		public virtual int SortOrder
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
		
		private int? _glyphiconID;
		public virtual int? GlyphiconID
		{
			get
			{
				return this._glyphiconID;
			}
			set
			{
				this._glyphiconID = value;
			}
		}
		
		private string _currentModulePattern;
		public virtual string CurrentModulePattern
		{
			get
			{
				return this._currentModulePattern;
			}
			set
			{
				this._currentModulePattern = value;
			}
		}
		
		private bool _showInMobileNav;
		public virtual bool ShowInMobileNav
		{
			get
			{
				return this._showInMobileNav;
			}
			set
			{
				this._showInMobileNav = value;
			}
		}
		
		private LinkGroup _linkGroup;
		public virtual LinkGroup LinkGroup
		{
			get
			{
				return this._linkGroup;
			}
			set
			{
				this._linkGroup = value;
			}
		}
		
		private Glyphicon _glyphicon;
		public virtual Glyphicon Glyphicon
		{
			get
			{
				return this._glyphicon;
			}
			set
			{
				this._glyphicon = value;
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
