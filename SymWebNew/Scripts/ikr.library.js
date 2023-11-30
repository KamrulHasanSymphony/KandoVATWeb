//www.youtube.com/ThumbIKR

var IkrHelper = {
    KendoNumberFormat(value) {
        return kendo.toString(value, "n2");
    },
    EmptyThenZero(ikrFieldValue) {
        if (isNaN(ikrFieldValue) || ikrFieldValue == "" || typeof ikrFieldValue === "undefined" || ikrFieldValue == "-" || ikrFieldValue == null) return 0;
        return ikrFieldValue;
    },
    SortArray(myArray, propName) {
        if (myArray.length > 0 && propName.trim().length > 0) return myArray.sort((a, b) => a[propName].localeCompare(b[propName]));
        return myArray;
    },
    GetResetDateKendo() {
        return kendo.parseDate(new Date(), "dd-MMM-yyyy");
    },
    DateFormat(paramDate) {
        return (new Date(paramDate)).toString('dd-MMM-yyyy');
    },
    IsValidEmail(email) {
        return String(email)
            .toLowerCase()
            .match(
                /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
            );
    },
}

var GroupBy = function (xs, key) {
    //How to use :     
    //var newList = groupBy(persons, "Country");
    return xs.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};

//Declaration : $.distinct([0,1,2,2,3]); outpur : 0,1,2,3
$.extend({
    distinct: function (anArray) {
        var result = [];
        $.each(anArray, function (i, v) {
            if ($.inArray(v, result) == -1) result.push(v);
        });
        return result;
    }
});
