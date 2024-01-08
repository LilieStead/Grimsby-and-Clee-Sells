function validatePrice(input) {
    // Remove any non-numeric characters except for the pound symbol (£)
    input.value = input.value.replace(/[^0-9£.]/g, '');

    // If the input starts with a dot, add a leading zero
    if (input.value.startsWith('.')) {
        input.value = '0' + input.value;
    }

    // If the input contains more than one dot, remove extra dots
    if (input.value.split('.').length > 2) {
        input.value = input.value.replace(/\.+$/, '');
    }

    // Limit to two decimal places
    input.value = input.value.replace(/(\.\d{2})\d+$/, '$1');
}