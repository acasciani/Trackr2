<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttendanceTrackingWidget.ascx.cs" Inherits="Trackr.Source.Controls.AttendanceTrackingWidget" %>

<script type="text/javascript">
    $(document).ready(function () {
        $(".attendance-ticker").click(function () {
            $.ajax({
                url: "test.html",
                context: document.body
            }).done(function () {
                $(this).addClass("done");
            });
        });
    })


</script>


<div class="panel panel-default">
                <div class="panel-body attendance-ticker">
                    Austin Jacobs <span class="glyphicon glyphicon-ok" style="float:right;"></span>
                </div>
</div>