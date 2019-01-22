var Website = function () {
    var _baseUrl;

    this.Init = function (base_url) {
        _baseUrl = base_url;
    };

    this.SiteUrl = function (url) {
        if (isEmpty(url)) return _baseUrl;

        return _baseUrl + url;
    };

    this.IsEmpty = function (value) {
        return (value === undefined) || (value === null) || (value === "");
    };

    this.AjaxUrl = function (url, type, data) {
        type = !this.IsEmpty(type) ? type : {};
        data = !this.IsEmpty(data) ? data : {};

        return $.ajax({
            url: url,
            type: type,
            data: data,
            dataType: "json"
        });
    };

    this.UrlParam = function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (results === null) {
            return null;
        }
        else {
            return decodeURI(results[1]) || 0;
        }
    };
};

var Datagrid = function (title, idField, url, queryParams, pageSize = 100) {
    queryParams = !website.IsEmpty(queryParams) ? queryParams : {};

    var selected = loaded = false, filterValue = {};
    var $dataGrid = $("#DataGrid");

    this.dataInit = {
        title: title,
        idField: idField,
        url: url,
        queryParams: queryParams,
        width: "100%",
        checkOnSelect: false,
        selectOnCheck: false,
        singleSelect: false,
        rowStyler: function (index, row) {
            if (!website.IsEmpty(row.IsFilter)) return { class: 'filterColumn' }
        },
        onLoadSuccess: function (data) {
            $dataGrid.datagrid('uncheckAll');
            $('div.datagrid-body').unbind('dblclick');

            $dataGrid.datagrid('insertRow', {
                index: 0,
                row: setFilterColumn(filterValue)
            });

            if (data.total > 0) {
                $('.datepicker').datepicker({
                    dateFormat: 'yy-mm-dd'
                });

                $('.filterColumn').on('change', 'input, select', function () {
                    if (!$(this).is(':checkbox')) {
                        filterValue = getFilterData();

                        $dataGrid.datagrid('reload');
                    }
                });
            } else {
                $dataGrid.datagrid('deleteRow', 0);
            }
        },
        onBeforeLoad: function (param) {
            Object.keys(filterValue).forEach(function (key) {
                if (!website.IsEmpty(filterValue[key])) param[key] = filterValue[key];
            });
        },
        onBeforeSelect: selectAction,
        onBeforeUnselect: selectAction,
        onBeforeCheck: function (index, row) { return website.IsEmpty(row.IsFilter); },
        onCheck: onCheck,
        onUncheck: onUncheck,
        onCheckAll: onCheckAll,
        onUncheckAll: onUncheckAll,
        pagination: true,
        pagePosition: "bottom",
        pageSize: pageSize
    }

    function selectAction(index, row) {
        if (selected) {
            selected = false;
            return true;
        }
        return false;
    }
    
    function onCheck(index, row) {
        selected = true;
        $dataGrid.datagrid('selectRow', index);
    }

    function onUncheck(index, row) {
        selected = true;
        $dataGrid.datagrid('unselectRow', index);
    }

    function onCheckAll(rows) {
        $dataGrid.datagrid('selectAll');
    }

    function onUncheckAll(rows) {
        $dataGrid.datagrid('unselectAll');
    }

    this.setInputText = function (ID, field, value) {
        return "<input type='text' class='form-control input-sm' id='" + field + "' value='" + $.trim(value) + "' onchange='onDataChange(\"" + ID + "\")'>";
    };

    this.setDropdown = function (ID, field, option, value) {
        return "<select class='form-control input-sm' id='" + field + "' onchange='onDataChange(\"" + ID + "\")'>" + this.setSelectOption(option, value) + "</select>";
    };
    this.setDropdownDisabled = function (ID, field, option, value) {
        return "<select class='form-control input-sm' id='" + field + "' onchange='onDataChange(\"" + ID + "\")' disabled>" + this.setSelectOption(option, value) + "</select>";
    };
    this.setCheckBox = function (ID, field, value) {
        var name = field;
        return "<input type='checkbox' class='form-control' id='" + name + "' " + (value ? "checked" : "") + " onchange='onDataChange(\"" + ID + "\")'>"
    };

    this.setSelectOption = function (optionList, value) {
        var option = "";
        if (optionList.length > 0) {
            for (var i in optionList) {
                option += !website.IsEmpty(optionList[i]) ? "<option value='" + optionList[i]["value"] + "' " + (!website.IsEmpty(value) && optionList[i]["value"] === value ? "selected" : "") + ">" + optionList[i]["text"] + "</option>" : "";
            }
        }

        return option;
    };
};