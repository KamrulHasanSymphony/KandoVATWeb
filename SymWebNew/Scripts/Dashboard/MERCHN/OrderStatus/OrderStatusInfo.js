
var OrderStatusManager = {
    GetOrderStatusGraphData: function (userId) {
        var objOrdSt = "";
        var jsonParam = "";
        var serviceUrl = _baseUrl + "/api/Dashboards/GetOrderStatusGraph/" + userId ;
        AjaxManager.GetJsonResult(serviceUrl, jsonParam, false, false, onSuccess, onFailed);
        function onSuccess(jsonData) {
            objOrdSt = jsonData;
        }
        function onFailed(error) {
            window.alert(error.statusText);
        }
        return objOrdSt;
    }
};

var OrderStatusHelper = {
    InitOrderStatus: function () {
        OrderStatusHelper.GenerateOrderStatusChart();
        //OrderStatusHelper.GenerateOrderStatusChart5();
    },
    GenerateOrderStatusChart: function () {
        var data = OrderStatusManager.GetOrderStatusGraphData(CurrentUser.USERID);
        var chart = AmCharts.makeChart("grphOrderStatus", {
            "type": "serial",
            "theme": "none",
            "marginRight": 70,
            "dataProvider": data,
            "valueAxes": [{
                "axisAlpha": 0,
                "position": "left",
                "title": "Purchase Order Quantity"
            }],
            //"legend": {
            //    "useGraphSettings": true
            //},
            "startDuration": 1,
            "graphs": [{
                "balloonText": "<b>Projection: [[value]]</b>",
                "fillColorsField": "color",
                "fillAlphas": 0.9,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "ProjectionQty",
                "labelText": "Projection: [[ProjectionQty]]"
            },
            {
                "balloonText": "<b>Confirm: [[value]]</b>",
                "fillColorsField": "color",
                "fillAlphas": 0.9,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "ConfirmedQty",
                "labelText": "Confirm: [[ConfirmedQty]]"
            },
            {
                "balloonText": "<b>Shiped: [[value]]</b>",
                "fillColorsField": "color",
                "fillAlphas": 0.9,
                "lineAlpha": 0.2,
                "type": "column",
                "valueField": "ShipedQty",
                "labelText": "Shiped: [[ShipedQty]]"
            }],
            "chartCursor": {
                "categoryBalloonEnabled": false,
                "cursorAlpha": 0,
                "zoomable": false
            },
            "categoryField": "ShipmentDate",
            "categoryAxis": {
                "gridPosition": "start",
                "labelRotation": 45
            },
            "export": {
                "enabled": true
            }

        });
    },
    //AmChart5
    GenerateOrderStatusChart5: function () {
        debugger;
        var data = OrderStatusManager.GetOrderStatusGraphData(CurrentUser.USERID);
        am5.ready(function () {
            var root = am5.Root.new("grphOrderStatus2");
            // Set themes
            // https://www.amcharts.com/docs/v5/concepts/themes/
            root.setThemes([
              am5themes_Animated.new(root)
            ]);

            // Create chart
            // https://www.amcharts.com/docs/v5/charts/xy-chart/
            var chart = root.container.children.push(am5xy.XYChart.new(root, {
                panX: false,
                panY: false,
               // wheelX: "panX",
                //wheelY: "zoomX",
                layout: root.verticalLayout
            }));


            // Add legend
            // https://www.amcharts.com/docs/v5/charts/xy-chart/legend-xy-series/
            var legend = chart.children.push(
              am5.Legend.new(root, {
                  centerX: am5.p50,
                  x: am5.p50
              })
            );

            // Create axes
            // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
            var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
                categoryField: "ShipmentDate",
                renderer: am5xy.AxisRendererX.new(root, {
                    cellStartLocation: 0.1,
                    cellEndLocation: 0.9
                }),
                tooltip: am5.Tooltip.new(root, {})
            }));

            xAxis.data.setAll(data);

            var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
                renderer: am5xy.AxisRendererY.new(root, {})
            }));


            // Add series
            // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
            function makeSeries(name, fieldName) {
                var series = chart.series.push(am5xy.ColumnSeries.new(root, {
                    name: name,
                    xAxis: xAxis,
                    yAxis: yAxis,
                    valueYField: fieldName,
                    categoryXField: "ShipmentDate"
                }));

                series.columns.template.setAll({
                    tooltipText: "{name}:{valueY}",
                    width: am5.percent(90),
                    tooltipY: 0
                });

                series.data.setAll(data);

                // Make stuff animate on load
                // https://www.amcharts.com/docs/v5/concepts/animations/
                series.appear();

                series.bullets.push(function () {
                    return am5.Bullet.new(root, {
                        locationY: 0,
                        sprite: am5.Label.new(root, {
                            text: "{valueY}",
                            fill: root.interfaceColors.get("alternativeText"),
                            centerY: 0,
                            centerX: am5.p50,
                            populateText: true
                        })
                    });
                });
                
                legend.data.push(series);
            }

            makeSeries("Projection", "ProjectionQty");
            makeSeries("Confirmed", "ConfirmedQty");
            makeSeries("Shiped", "ShipedQty");

            chart.appear(1000, 100);

        }); // end am5.ready()
    },
};