function displayFileNames() {
    const fileInput = document.getElementById('fileInput');
    const fileNamesContainer = document.getElementById('fileNames');
    fileNamesContainer.innerHTML = '';

    if (fileInput.files.length > 0) {
        Array.from(fileInput.files).forEach((file, index) => {
            const fileNameElement = document.createElement('div');
            fileNameElement.textContent = `${index + 1}. ${file.name}`;
            fileNamesContainer.appendChild(fileNameElement);
        });
    } else {
        fileNamesContainer.textContent = 'No files selected';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    const fileList = document.getElementById('fileList');
    const notificationContainer = document.getElementById('notificationContainer');

    function resetForm() {
        const form = document.getElementById('uploadForm');
        form.reset(); 

        const fileNamesDiv = document.getElementById('fileNames');
        fileNamesDiv.innerHTML = ''; 
    }

    $('#uploadForm').submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);
        formData.append('files', $('#fileInput')[0].files[0]);

        $.ajax({
            url: this.action,
            type: this.method,
            data: formData,
            processData: false,
            contentType: false,
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                displayNotification('File uploaded successfully', 'success');
                resetForm();
                fetchFiles();
            },
            error: function (xhr, status, error) {
                displayNotification('Error uploading file', 'error');
            }
        });
    });

    async function fetchFiles() {
        try {
            const response = await fetch('/api/files/listFiles');
            if (!response.ok) {
                fileList.innerHTML = '<p>Error loading files. Please try again later.</p>';
            }

            const files = await response.json();
            displayFiles(files);
        } catch (error) {
            fileList.innerHTML = '<p>Error loading files. Please try again later.</p>';
        }
    }

    function displayFiles(files) {
        fileList.innerHTML = ''; 
        if (files.length === 0) {
            fileList.innerHTML = '<p>No files available.</p>';
            return;
        }

        files.forEach(file => {
            const listItem = document.createElement('li');
            listItem.className = 'file-item';

            listItem.innerHTML =
                `<div class="file-info">
                    <i>&#128194;</i>
                    <div>
                        <span>${file.fullName}</span>
                        <div class="file-date">Uploaded on: ${file.uploadedOn}</div>
                    </div>
                </div>
                <a href="/api/files/download/${file.id}" download>
                    <button class="download-btn">Download</button>
                </a>
                <button class="delete-btn" onclick="deleteFile('${file.id}', this)">Delete</button>`;

            fileList.appendChild(listItem);
        });
    }

    function displayNotification(message, type) {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;

        notificationContainer.innerHTML = '';
        notificationContainer.appendChild(notification);

        setTimeout(() => {
            notificationContainer.innerHTML = '';
        }, 3000);
    }

    function deleteFile(fileId, button) {
        if (!confirm("Are you sure you want to delete this file?")) {
            return;
        }

        $.ajax({
            url: `/api/files/delete/${fileId}`,
            type: 'DELETE',
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            },
            success: function (response) {
                const listItem = $(button).closest('.file-item');
                listItem.remove();
                displayNotification('File deleted successfully.', 'success');
            },
            error: function (xhr, status, error) {
                displayNotification('An error occurred while deleting the file. Please try again.', 'error');
            }
        });
    }

    fetchFiles();
});
