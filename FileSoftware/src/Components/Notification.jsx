import React, { useEffect } from 'react';

const Notification = ({ message, type, clearNotification }) => {
    useEffect(() => {
        if (message) {
            const timer = setTimeout(() => {
                clearNotification();
            }, 3000);
            return () => clearTimeout(timer); // Cleanup timeout
        }
    }, [message, clearNotification]);

    if (!message) return null;

    return <div className={`notification ${type}`}>{message}</div>;
};

export default Notification;
