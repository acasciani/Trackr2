function bounceProgressAttach(selectorOverlay, selectorBounce) {
    function resize() {
        var height = $(selectorOverlay).parent().parent().parent().height();
        $(selectorOverlay).height(height);
        $(selectorBounce).height(height);
        $(selectorBounce).css('padding-top', (height / 2) - 11);
    }

    resize();

    $(window).resize(function () {
        resize();
    });
}