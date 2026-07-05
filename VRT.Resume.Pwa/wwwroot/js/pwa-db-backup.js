// Download SQLite backup bytes and trigger hidden file picker for import.
window.pwaDbBackup = {
    downloadFile: function (fileName, base64Data) {
        const binary = atob(base64Data);
        const bytes = new Uint8Array(binary.length);
        for (let i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        const blob = new Blob([bytes], { type: 'application/octet-stream' });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName;
        anchor.click();
        URL.revokeObjectURL(url);
    },

    openFilePicker: function (inputId) {
        const input = document.getElementById(inputId);
        if (input) {
            input.value = '';
            input.click();
        }
    },
};