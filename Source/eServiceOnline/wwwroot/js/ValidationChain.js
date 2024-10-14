var ValidationsChain = (function () {
    /**
     * adds a function to chain a validation
     * @param  {Boolean} defaultResult Default value to return in the case the function (validation) does not return anything
     * @param  {function|Object} validation function or Object with validation and arguments
     * @param  {function} when (optional) function to decide if the validation function is executed or not, it receives an object with true or false if the validation has already byPassed and the validation state
     * @param  {function} breakChainOn (optional) function to decide if the chain validations continues execution or returns the default current validation value
     * @param  {function} stateDataFn (optional) function to call in the case of save status function data
     * @return {ValidationsChain} new validation chain object
     */
    function ValidationsChain(defaultResult, validation, when, breakChainOn, stateDataFn) {
        if (when && typeof when !== "function") {
            throw "when must be function";
        }

        if (typeof validation !== "object" && typeof validation !== "function") {
            throw "validation must be function or object with func and args";
        }

        if (typeof defaultResult !== "boolean") {
            throw "defaultResult must be boolean";
        }

        if (breakChainOn && typeof breakChainOn !== "function") {
            throw "breakChainOn must be a function";
        }

        if (stateDataFn && typeof stateDataFn !== "function") {
            throw "stateDataFn must be a function";
        }

        this.result;
        this.defaultResult = defaultResult;
        this.next = null;
        this.validation = validation;
        this.when = when;
        this.breakChainOn = breakChainOn;
        this.bypassed = undefined;
        this.stateData = undefined;
        this.stateDataFn = stateDataFn;
    }

    function convertObjectToArrary(obj) {
        return Object.keys(obj).map(function (key) {
            return obj[key];
        });
    }

    var getParameters = function (objOrFunc) {
        var array;

        if (Array.isArray(objOrFunc)) {
            return objOrFunc;
        }

        if (typeof objOrFunc === "object") {
            array = convertObjectToArrary(objOrFunc);
        }
        else {
            array = getParameters(objOrFunc.call(objOrFunc));
        }

        return array;
    }

    var isSameStateData = function(stateData, dataFunction) {
        var cachedData = "";
        var currentData = "";

        if (typeof stateData == "object") {
            cachedData = JSON.stringify(stateData);
            currentData = JSON.stringify(dataFunction.call());
        } else {
            cachedData = stateData;
            currentData = dataFunction.call();
        }

        return cachedData === currentData;
    }

    ValidationsChain.prototype = {
        validate: function () {
            if (this.breakChainOn && this.breakChainOn.call()) {
                return this.defaultResult;
            }

            if (this.when && !this.when.call(undefined, { bypassed: this.bypassed, currentStateData: this.stateData })) {
                this.result = this.defaultResult;
            }
            else {

                var isBypassed = false;
                if (this.bypassed) {
                    if (this.stateDataFn) {
                        if (isSameStateData(this.stateData, this.stateDataFn)) {
                            isBypassed = true;
                            this.result = true;
                        } else {
                            this.bypassed = undefined;
                        }
                    } else {
                        isBypassed = true;
                        this.result = true;
                    }
                }

                if (!isBypassed) {
                    if (typeof this.validation === 'function') {
                        this.result = this.validation.call();
                    } else if (typeof this.validation === 'object') {
                        this.result = this.validation.func.apply(this.validation.func, getParameters(this.validation.args));
                    }
                }

                if (this.result === undefined || this.result === null) {
                    this.result = this.defaultResult;
                }
            }

            console.log("Validation:" + (this.validation.hasOwnProperty("name") && this.validation.name) + "; result:" + this.result);

            if (this.result === true) {
                if(this.next)
                    return this.next.validate();
            } else {
                return false;
            }

            return this.result;
        },
        /**
         * Sets the next Validation chain object
         * @param {function} chainVal
         */
        setNextFuncOrValidation: function (chainVal) {
            this.next = chainVal;
        },

        /**
         *Sets the function as bypassed
         * @param {bool} passVal
         */
        setPass: function (passVal) {
            if (passVal) {
                this.stateData = this.stateDataFn ? this.stateDataFn.call() : undefined;
            }

            this.bypassed = passVal;
        }
    }

    return ValidationsChain;
}());

var ValidationsBuilder = (function () {
    function ValidationsBuilder(chainedValidations) {
        this.validations = chainedValidations;
    }

    ValidationsBuilder.prototype.validate = function () {
        return this.validations.validate();
    }
    return ValidationsBuilder;
}());