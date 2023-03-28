function scrollToEnd(div) {
    div.scrollTop = div.scrollHeight;
}
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
(function () {
    window.QuillFunctions = {
        createQuill: function (quillElement) {
            var options = {
                debug: 'info',
                modules: {
                    toolbar: '#toolbar'
                },
                placeholder: '',
                readOnly: false,
                theme: 'snow'
            };
            // set quill at the object we can call
            // methods on later
            new Quill(quillElement, options);
        },
        getQuillContent: function (quillControl) {
            if (quillControl && quillControl.__quill)
                return JSON.stringify(quillControl.__quill.getContents());
            return null;
        },
        getQuillText: function (quillControl) {
            if (quillControl && quillControl.__quill)
                return quillControl.__quill.getText();
            return null;
        },
        getQuillHTML: function (quillControl) {
            if (quillControl && quillControl.__quill)
                return quillControl.__quill.root.innerHTML;
            return null;
        },
        loadQuillContent: function (quillControl, quillContent) {
            if (quillControl && quillControl.__quill) {
                content = JSON.parse(quillContent);
                return quillControl.__quill.setContents(content, 'api');
            }
            return null;
        },
        setQuillContent: function (quillControl, quillContent) {
            if (quillControl && quillControl.__quill && quillControl.__quill.root) {
                quillControl.__quill.root.innerHTML = quillContent;
            }
        },
        disableQuillEditor: function (quillControl) {
            if (quillControl && quillControl.__quill) {
                quillControl.__quill.enable(false);
                quillControl.__quill.root.innerHTML = "";
            }
        }
    };
    window.CanvasFunctions = {
        resize: function () {
            var div = document.getElementById('wordCloudSection');
            var canvas = document.querySelector('canvas');
            if (canvas) {
                canvas.style.width = '100%';
                canvas.width = div.offsetWidth;
            }
            return div.offsetWidth
        }
    };

})();