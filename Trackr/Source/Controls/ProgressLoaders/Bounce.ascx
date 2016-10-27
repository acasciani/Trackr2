<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Bounce.ascx.cs" Inherits="Trackr.Controls.ProgressLoaders.Bounce" %>

<div style="position: relative;">
    <div style="width: 100%; height: 220px; position: absolute; background-color: rgba(255,255,255, 0.85); z-index: 5000;" runat="server" id="divOverlay">
    </div>

    <div style="width: 100%; height: 220px; position: absolute; z-index: 5001; padding-top: 101px;" runat="server" id="divBounce">
        <div class="loading spinner">
            <div class="bounce1"></div>
            <div class="bounce2"></div>
            <div class="bounce3"></div>
        </div>
    </div>

</div>

