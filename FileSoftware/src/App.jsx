import React, { useState, useEffect } from 'react';
import FileUploader from './Components/FileUploader';
import FileList from './Components/FileList';
import Notification from './Components/Notification';

const App = () => {
    const [fileList, setFileList] = useState([]);
    const [notification, setNotification] = useState({ message: '', type: '' });

    useEffect(() => {
        fetchFiles();
    }, []);

    // Fetch files from the server
    const fetchFiles = async () => {
        try {
            const response = await fetch('/api/files/listFiles');
            if (response.ok) {
                const data = await response.json();
                setFileList(data);
            } else {
                displayNotification('Error loading files. Please try again later.', 'error');
            }
        } catch (error) {
            displayNotification('Error loading files. Please try again later.', 'error');
        }
    };

    // Handle file upload
    const handleFileUpload = async (files, csrfToken) => {
        const formData = new FormData();
        Array.from(files).forEach(file => formData.append('files', file));
       
        try {
            const response = await fetch('/api/files/upload', {
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': csrfToken,
                },
                body: formData,
            });

            if (response.ok) {
                displayNotification('File uploaded successfully', 'success');
                fetchFiles();
            } else {
                displayNotification('Error uploading file', 'error');
            }
        } catch (error) {
            displayNotification('Error uploading file', 'error');
        }
    };

    // Handle file deletion
    const handleDelete = async (fileId, csrfToken) => {
        if (!window.confirm('Are you sure you want to delete this file?')) {
            return;
        }

        try {
            const response = await fetch(`/api/files/${fileId}`, {
                method: 'DELETE',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest',
                    'X-CSRF-TOKEN': csrfToken,
                },
            });

            if (response.ok) {
                displayNotification('File deleted successfully', 'success');
                fetchFiles();
            } else {
                displayNotification('Error deleting file. Please try again.', 'error');
            }
        } catch (error) {
            displayNotification('Error deleting file. Please try again.', 'error');
        }
    };

    // Display notification
    const displayNotification = (message, type) => {
        setNotification({ message, type });
    };

    const clearNotification = () => {
        setNotification({ message: '', type: '' });
    };

    return (
        <div>
            <h2>File Management</h2>

            <Notification
                message={notification.message}
                type={notification.type}
                clearNotification={clearNotification}
            />

            <div className="flexed-container">
                <FileUploader onFileUpload={handleFileUpload} />
                <FileList files={fileList} onDelete={handleDelete} />
            </div>
        </div>
    );
};

export default App;
