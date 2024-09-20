import React from 'react';
import FileItem from './FileItem';

const FileList = ({ files, onDelete }) => {
    return (
        <div class="file-list-container">
            <h2>Your Files</h2>
            <ul id="fileList">
                {files.length > 0 ? (
                    files.map(file => (
                        <FileItem key={file.id} file={file} onDelete={onDelete} />
                    ))
                ) : (
                    <li>No files available</li>
                )}
            </ul>
        </div>
    );
};

export default FileList;
