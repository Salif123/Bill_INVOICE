window.downloadPDF = function (base64Data, fileName) {
    console.log("Download initiated for:", fileName);
    try {
        const link = document.createElement('a');
        link.href = 'data:application/pdf;base64,' + base64Data;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        setTimeout(() => document.body.removeChild(link), 100);
        return true;
    } catch (error) {
        console.error("Download failed:", error);
        return false;
    }
};
