import React, { useState, useEffect } from 'react';

const FileItem = ({ file, onDelete }) => {

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

    return (
        <li className="file-item">
            <div className="file-info">
                <i>&#128194;</i>
                <div>
                    <span>{file.fullName}</span>
                    <div className="file-date">Uploaded on: {file.uploadedOn}</div>
                </div>
            </div>
            <div className="file-buttons">
                <a href={`/api/files/download/${file.id}`} download>
                    <button className="download-btn">Download</button>
                </a>
                <button className="delete-btn" onClick={() => onDelete(file.id, csrfToken)}>Delete</button>
            </div>
        </li>
    );
};

export default FileItem;
