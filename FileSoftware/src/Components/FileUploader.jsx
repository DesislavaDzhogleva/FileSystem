import React, { useState, useEffect } from 'react';

const FileUpload = ({ onFileUpload }) => {
    const [selectedFiles, setSelectedFiles] = useState([]);
    const [fileNames, setFileNames] = useState([]);
    const [csrfToken, setCsrfToken] = useState('');

    const getCsrfToken = async () => {
        const response = await fetch('/api/csrf/token');
        const data = await response.json();
        return data.token;
    };

    useEffect(() => {
        const fetchToken = async () => {
            const token = await getCsrfToken();
            setCsrfToken(token);
        };

        fetchToken();
    }, []); 

    const handleFileChange = (e) => {
        const files = Array.from(e.target.files);
        setSelectedFiles(files);
        setFileNames(files.map((file) => file.name));
    };

    const handleFormSubmit = async (e) => {
        e.preventDefault();
        onFileUpload(selectedFiles, csrfToken);
    };

    return (
        <div className="upload-container">
            <h2>Upload Your Files</h2>
            <form
                id="uploadForm"
                action="/api/files/upload"
                method="POST"
                encType="multipart/form-data"
                onSubmit={handleFormSubmit}>
                <div className="file-drop-area">
                    <p><i>⇪</i> Drag & Drop your files here</p>
                    <input
                        type="file"
                        id="fileInput"
                        name="files[]"
                        multiple
                        onChange={handleFileChange}
                    />
                </div>

                <div className="or-divider">OR</div>

                <label htmlFor="fileInput" className="custom-file-label">
                    Browse Files
                </label>

                <div id="fileNames">
                    {fileNames.length > 0 ? (
                        <ul>
                            {fileNames.map((fileName, index) => (
                                <li key={index}>{fileName}</li>
                            ))}
                        </ul>
                    )}
                </div>

                <button type="submit" className="upload-btn">
                    Upload Files
                </button>
            </form>
        </div>
    );
};

export default FileUpload;
