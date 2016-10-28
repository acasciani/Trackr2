function bounceProgressAttach(selectorOverlay, selectorBounce) {
    function resize() {
        var height = $(selectorOverlay).parent().parent().parent().height();
        $(selectorOverlay).height(height);
       // $(selectorBounce).height(23);
        $(selectorBounce).css('padding-top', (height / 2) - 11);
    }

    resize();

    $(window).resize(function () {
        resize();
    });
}