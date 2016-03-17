$(document).ready(function () {
    $('.attendance-ticker').click(function () { MarkPresent(); });

    function MarkPresent(playerID, teamScheduleID) {

        $.post('/api/Attendances/CreateTokenTemp').done(function (token) {
            $.post('/api/Attendances/MarkPresent', { PlayerID: playerID, TeamScheduleID: teamScheduleID, Token: token })
                .done(function () {
                    $(this).addClass('done');
                })
                .fail(function (v) {
                    console.log('failed');
                });
        })
            .fail(function (v) {
                console.log('failed to generate token');
            });
    }

});