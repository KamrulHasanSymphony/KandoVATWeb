$.fn.kendoEmailAddressSelect = function (options) {
    var multiSelect = $(this).kendoMultiSelect(options).getKendoMultiSelect();

    //$(this).keypress(function (e) {
    //    if (e.keyCode == '13') {
    //        debugger;
    //        console.log(multiSelect.values());
    //    }
    //});

    multiSelect.bind('filtering', function (ev) {
        debugger;
        var val = ev.filter && ev.filter.value;
        if (!val) return;
        var keyCode = val.charCodeAt(0);
        if (keyCode != 59) {//semicolon
            return;
        }
       
        var dataSource = ev.sender.dataSource;
        var items = dataSource.data();

        // if there is an existing item in the list, don't create a new one
        var existingItem = items.filter(function (i) {
            return i.value == val;
        })[0];
        if (existingItem) return;

        // find or create the item that will hold the current filter value
        var inputItem = items.filter(function (i) {
            return i.isInput;
        })[0];
        if (!inputItem) {
            inputItem = dataSource.insert(0, { isInput: true });
            // when inserting a value the input gets cleared in some situations
            // so set it back 
            ev.sender.input.val(ev.filter.value);
        }
        inputItem.value = val;
    });

    // cleans input items and also applies an optional value transformation function
    var updateValues = function (ev) {
        debugger;
        var values = ev.sender.value();
        if (typeof options.valueTransformationFunction === 'function') {
            // for example split comma separated values
            values = options.valueTransformationFunction(values);
        }

        var dataSource = ev.sender.dataSource;
        var items = dataSource.data();
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            item.shouldBeKept = false;
        }

        // add items for existing values
        for (var i = 0; i < values.length; i++) {
            var value = values[i];

            var item = items.filter(function (i) { return i.value == value; })[0];
            if (!item) {
                item = dataSource.add({ value: value });
            }
            item.isInput = false;
            item.shouldBeKept = true;
        }

        ev.sender.value(values);

        // delete all others
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (!item.shouldBeKept) {
                dataSource.remove(item);
            }
        }
    };

    //multiSelect.bind('change', updateValues);
    //multiSelect.bind('close', updateValues);
};
