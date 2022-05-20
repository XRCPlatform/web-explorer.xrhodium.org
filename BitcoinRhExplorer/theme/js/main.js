/**
 ******************************
 Page Scripts
 ******************************
 */

var dashboardPage = function() {
	var total_chart = {
		size:      110,
		animate:   App.is_old_ie ? false : 2000,
		lineWidth: 6,
		lineCap:   'square'
	};
	
var txout_chart = {
    data: [{color:'#f3cf73', label:'Amount in XRC', data:txout_datagrid}],
    options: {
        grid: {
            borderWidth: 1,
            borderColor: '#CCCCCC',
            clickable: true,
            hoverable: false
        },
        xaxis: {
			min: txout_min
        },
        yaxis: {
            min: 0,
			max: txout_max
        },
        series: {
            lines: {
                show: true
            },
            points: {
                show: false
            }
        },
        lines: {
            lineWidth: 2,
            fill:      true,
            fillColor: {
                colors: [{opacity:0.4}, {opacity:0.9}]
            }
        },
        shadowSize: 2
    }
};
	
var diff_chart = {
    data: [
        {color:'#44A340', label:'Difficult',  data:diff_datagrid}
    ],
    options: {
        grid: {
            borderWidth: 1,
            borderColor: '#CCCCCC',
            clickable: true,
            hoverable: true
        },
        tooltip: true,
        tooltipOpts: {
            content:      '%x: %y',
            defaultTheme: false
        },
        crosshair: {
            mode: 'x'
        },
        xaxis: {
            min: 0
        },
        yaxis: {
            min: 0
        },
        series: {
            lines: {
                show: true
            },
            points: {
                show: true
            }
        },
        lines: {
            lineWidth: 1
        },
        shadowSize: 2
    }
};

	
    // line chart
    var plot = $.plot($('#dashboard-line-chart'), diff_chart.data, diff_chart.options);

    var circular_pies = $('div.circular-pie');
    circular_pies.each(function() {
        var chart = $(this);
        var text_span = chart.children('span');
        chart.easyPieChart($.extend(true, {}, total_chart, {
            barColor:   chart.data('color'),
            lineWidth:  15,
            trackColor: '#FFFFFF',
            scaleColor: false,
            onStep: function(from, to, currentValue) {
                text_span.text(parseInt(currentValue, 10) + '%');
            }
        }));
    });
					
    var legends = $('#dashboard-line-chart .legendLabel');

    var updateLegendTimeout = null;
    var latestPosition      = null;

    var updateLegend = function() {
        updateLegendTimeout = null;

        var pos = latestPosition;

        var axes = plot.getAxes();
        if (pos.x < axes.xaxis.min || pos.x > axes.xaxis.max ||
            pos.y < axes.yaxis.min || pos.y > axes.yaxis.max) {
            return;
        }

        var dataset = plot.getData();
        for (var i = 0; i < dataset.length; ++i) {

            var series = dataset[i];

            // Find the nearest points, x-wise
            for (var j = 0; j < series.data.length; ++j) {
                if (series.data[j][0] > pos.x) {
                    break;
                }
            }

            // Now Interpolate
            var y,
                p1 = series.data[j - 1],
                p2 = series.data[j];

            if (p1 == null) {
                y = p2[1];
            } else if (p2 == null) {
                y = p1[1];
            } else {
                y = p1[1] + (p2[1] - p1[1]) * (pos.x - p1[0]) / (p2[0] - p1[0]);
            }

            legends.eq(i).html('<table><tr><td width="50">' + series.label + ':</td><td>' + parseInt(y, 10) + '</td></tr></table>');
        }
    }

    $('#dashboard-line-chart').on('plothover',  function (event, pos, item) {
        latestPosition = pos;
        if (!updateLegendTimeout) {
            updateLegendTimeout = setTimeout(updateLegend, 50);
        }
    });

    $.plot($("#dashboard-area-chart"), txout_chart.data, txout_chart.options);
	
    $(document).ready(function() {
        App.closeLoading();
		
		$('#cn-accept-cookie').click(function() {
			setCookie('accept-cookie', 'true');
			$('#cookie-notice').removeClass('cookie-notice-visible').addClass('cookie-notice-hidden');
		});
	
		var acceptcookie = getCookie('accept-cookie');
		if (acceptcookie == 'true') {
			$('#cookie-notice').removeClass('cookie-notice-visible').addClass('cookie-notice-hidden');
		}
		
		$('#cn-decline-cookie').click(function() {
			window['ga-disable-G-EWCLLE92W5'] = true;
			if (window.ga) window.ga('remove');
			if (document.cookie) {
				var cookies = document.cookie.split(";");

				for (var i = 0; i < cookies.length; i++) {
					var cookie = cookies[i];
					var eqPos = cookie.indexOf("=");
					var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
					var domain = document.domain;
					document.cookie = name + "=;path=/;domain=" + domain + ";expires=Thu, 01 Jan 1970 00:00:00 GMT";
				document.cookie = name + "=;path=/;domain=" + domain.replace('explorer.', '') + ";expires=Thu, 01 Jan 1970 00:00:00 GMT";
					
				}
			}
			alert("Cookies have been removed.");
		});
    });	
};

var subPage = function() {
    $(document).ready(function() {
        App.closeLoading();

		$('#cn-accept-cookie').click(function() {
			setCookie('accept-cookie', 'true');
			$('#cookie-notice').removeClass('cookie-notice-visible').addClass('cookie-notice-hidden');
		});
	
		var acceptcookie = getCookie('accept-cookie');
		if (acceptcookie == 'true') {
			$('#cookie-notice').removeClass('cookie-notice-visible').addClass('cookie-notice-hidden');
		}
		
		$('#cn-decline-cookie').click(function() {
			window['ga-disable-G-EWCLLE92W5'] = true;
			if (window.ga) window.ga('remove');
			if (document.cookie) {
				var cookies = document.cookie.split(";");

				for (var i = 0; i < cookies.length; i++) {
					var cookie = cookies[i];
					var eqPos = cookie.indexOf("=");
					var name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
					var domain = document.domain;
					document.cookie = name + "=;path=/;domain=" + domain + ";expires=Thu, 01 Jan 1970 00:00:00 GMT";
				document.cookie = name + "=;path=/;domain=" + domain.replace('explorer.', '') + ";expires=Thu, 01 Jan 1970 00:00:00 GMT";
					
				}
			}
			alert("Cookies have been removed.");
		});
    });	
};

		function setCookie(key, value) {
			var expires = new Date();
			expires.setTime(expires.getTime() + 31536000000); 
			document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
		}

		function getCookie(key) {
			var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
			return keyValue ? keyValue[2] : null;
		}  