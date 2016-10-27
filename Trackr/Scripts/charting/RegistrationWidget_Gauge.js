var registrationProgressOptions = {

    chart: {
        type: 'solidgauge'
    },

    title: null,

    pane: {
        center: ['50%', '85%'],
        size: '140%',
        startAngle: -90,
        endAngle: 90,
        background: {
            backgroundColor: (Highcharts.theme && Highcharts.theme.background2) || '#EEE',
            innerRadius: '60%',
            outerRadius: '100%',
            shape: 'arc'
        }
    },

    tooltip: {
        enabled: false
    },

    // the value axis
    yAxis: {
        stops: [
            [0.1, '#55BF3B'], // green
            [0.5, '#DDDF0D'], // yellow
            [0.9, '#DF5353'] // red
        ],
        lineWidth: 0,
        minorTickInterval: null,
        tickAmount: 1,
        title: {
            y: -70
        },
    },

    plotOptions: {
        solidgauge: {
            dataLabels: {
                y: 5,
                borderWidth: 0,
                useHTML: true
            },

        }
    }
};

function UpdateGauge(element, title, value, min, max, valueLabel, valueSuffix) {
    // The speed gauge
    $(element).highcharts(Highcharts.merge(registrationProgressOptions, {
        yAxis: {
            min: min,
            max: max,
            title: {
                text: title
            },
            tickInterval: (max / 4)
        },

        credits: {
            enabled: false
        },

        series: [{
            name: title,
            data: [value],
            dataLabels: {
                format: '<div style="text-align:center"><span style="font-size:25px;color:' +
                    ((Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black') + '">{y}</span><br/>' +
                       '<span style="font-size:12px;color:silver">' + valueLabel + '</span></div>'
            },
            tooltip: {
                valueSuffix: ' ' + valueSuffix || valueLabel
            }
        }]

    }));
}