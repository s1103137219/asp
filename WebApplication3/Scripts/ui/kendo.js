//$(document).ready(function () {
//    $("#SearchResultSection").kendoGrid({
//        selectable: "multiple cell",
//        allowCopy: true
//    });
//    $("#SearchConditionSection").kendoGrid({
//        selectable: "multiple cell",
//        allowCopy: true
//    })
//});

$.getScript("http://kendo.cdn.telerik.com/2017.2.504/js/kendo.all.min.js", function () {
        $(".basicgrid").kendoGrid({
            selectable: "multiple cell",
            allowCopy: true
        });
        $(".basicbtn").kendoButton();
        $(".btnRemove").kendoButton();
})

