// Form validation for all contact forms
// Version: 2.2 - Cleaned up and fixed error message display
(function() {
    'use strict';

    //------------------------------------------------------------------------------------------------------
    // Initialize validation for all forms
    function initializeFormValidation() {
        const forms = document.querySelectorAll('form[id$="ContactForm"], form[id="contactForm"], form.phy-form, form.dt-cyber-contact-form, form.dt-contact-form');
        forms.forEach(form => {
            setupFormValidation(form);
        });
    }

    //------------------------------------------------------------------------------------------------------
    // Setup form validation for a specific form
    function setupFormValidation(form) {
        const sanitize = (v) => {
            if (!v) return v;
            return v.replace(/[<>]/g, '').trim();
        };
        const sanitizeMessage = (v) => {
            if (!v) return v;
            return v.replace(/[<>]/g, '');
        };
        
        // Rate limiting for security
        let submissionAttempts = 0;
        const maxAttempts = 5;
        const rateLimitWindow = 60000; // 1 minute
        let lastSubmissionTime = 0;

        const validators = {
            FirstName: (v) => {
                if (!v || v.length < 2) return { valid: false, message: 'First name must be at least 2 characters.' };
                if (v.length > 50) return { valid: false, message: 'First name cannot exceed 50 characters.' };
                if (!/^[A-Za-z\s]+$/.test(v)) return { valid: false, message: 'First name must contain only letters and spaces.' };
                return { valid: true };
            },
            LastName: (v) => {
                if (!v || v.length < 2) return { valid: false, message: 'Last name must be at least 2 characters.' };
                if (v.length > 50) return { valid: false, message: 'Last name cannot exceed 50 characters.' };
                if (!/^[A-Za-z\s]+$/.test(v)) return { valid: false, message: 'Last name must contain only letters and spaces.' };
                return { valid: true };
            },
            Email: (v) => {
                if (!v) return { valid: false, message: 'Email address is required.' };
                if (!v.includes('@')) return { valid: false, message: 'Email must contain @ symbol.' };
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(v)) return { valid: false, message: 'Email must be in valid format (e.g., user@example.com).' };
                if (v.length > 254) return { valid: false, message: 'Email cannot exceed 254 characters.' };
                return { valid: true };
            },
            PhoneNumber: (v) => {
                if (!v) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                const digitsOnly = v.replace(/[\s\-]/g, '');
                if (digitsOnly.length < 10) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                if (digitsOnly.length > 15) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                if (!/^\+?[0-9\s\-]+$/.test(v)) return { valid: false, message: 'Please enter a minimum of 10 digits.' };
                return { valid: true };
            },
            CompanyName: (v) => {
                if (v && v.length > 100) return { valid: false, message: 'Company name cannot exceed 100 characters.' };
                if (v && /[<>]/.test(v)) return { valid: false, message: 'Company name cannot contain < or > characters.' };
                return { valid: true };
            },
            Location: (v) => {
                if (!v) return { valid: false, message: 'Please select your location from the dropdown.' };
                return { valid: true };
            },
            ServiceId: (v) => {
                if (!v) return { valid: false, message: 'Please select a service from the dropdown.' };
                return { valid: true };
            },
            Message: (v) => {
                if (!v || v.length < 10) return { valid: false, message: 'Please enter a minimum of 10 characters.' };
                if (v.length > 1000) return { valid: false, message: 'Message cannot exceed 1000 characters.' };
                if (/[<>]/.test(v)) return { valid: false, message: 'Message cannot contain < or > characters.' };
                return { valid: true };
            }
        };

        //------------------------------------------------------------------------------------------------------
        // Find the invalid-feedback element for an input

        function findFeedbackElement(input) {
            const fieldName = input.getAttribute('name');
            if (!fieldName) return null;
            
            const feedbackId = fieldName.toLowerCase() + '-error';
            const feedbackById = document.getElementById(feedbackId);
            if (feedbackById && feedbackById.classList.contains('invalid-feedback')) {
                return feedbackById;
            }
            
            let sibling = input.nextElementSibling;
            while (sibling) {
                if (sibling.nodeType === Node.ELEMENT_NODE && sibling.classList && sibling.classList.contains('invalid-feedback')) {
                    return sibling;
                }
                sibling = sibling.nextElementSibling;
            }
            
            const wrapper = input.closest('.col-12');
            if (wrapper) {

                const children = Array.from(wrapper.children);
                const inputIndex = children.indexOf(input);
                if (inputIndex !== -1) {
                    for (let i = inputIndex + 1; i < children.length; i++) {
                        if (children[i].classList && children[i].classList.contains('invalid-feedback')) {
                            return children[i];
                        }
                    }
                }
                // Fallback: find first invalid-feedback in wrapper
                const feedbacks = wrapper.querySelectorAll('.invalid-feedback');
                if (feedbacks.length > 0) {
                    return feedbacks[0];
                }
            }
            
            // Last resort: check parent element for direct siblings
            const parent = input.parentElement;
            if (parent) {
                const children = Array.from(parent.children);
                const inputIndex = children.indexOf(input);
                if (inputIndex !== -1) {
                    for (let i = inputIndex + 1; i < children.length; i++) {
                        if (children[i].classList && children[i].classList.contains('invalid-feedback')) {
                            return children[i];
                        }
                    }
                }
            }
            
            return null;
        }

        //------------------------------------------------------------------------------------------------------
        function setInvalid(input, message) {
            input.classList.add('is-invalid');
            
            const feedback = findFeedbackElement(input);
            if (feedback) {
                feedback.textContent = message;
                feedback.style.display = 'block';
                feedback.style.visibility = 'visible';
            }
            
            input.setCustomValidity(message);
        }

        //------------------------------------------------------------------------------------------------------
        // Clear invalid state from an input
        function clearInvalid(input) {
            input.classList.remove('is-invalid');
            input.setCustomValidity('');
            
            const feedback = findFeedbackElement(input);
            if (feedback) {
                // Hide the error message
                feedback.style.display = 'block';
                feedback.style.visibility = 'hidden';
            }
        }

        //------------------------------------------------------------------------------------------------------
        // Display security error message
        function showSecurityError(message) {
            const alertDiv = document.createElement('div');
            alertDiv.className = 'alert alert-danger';
            alertDiv.innerHTML = '<strong>Security Alert:</strong> ' + message;
            form.parentNode.insertBefore(alertDiv, form);
            setTimeout(() => alertDiv.remove(), 5000);
        }

        //------------------------------------------------------------------------------------------------------
        // Prevent browser's default validation messages
        form.addEventListener('invalid', function(e) {
            e.preventDefault();
            e.stopPropagation();
            e.stopImmediatePropagation();
            const field = e.target;
            const name = field.getAttribute('name');
            if (validators[name]) {
                const val = field.value;
                const result = validators[name](val);
                if (!result.valid) {
                    setInvalid(field, result.message);
                }
            }
            return false;
        }, true);

        //--------------------------------------------------------------------------------------------------------
        // Add real-time validation to all form inputs
        const inputs = form.querySelectorAll('input, textarea, select');
        inputs.forEach(input => {
            // Remove title attribute to prevent browser tooltips
            if (input.hasAttribute('title')) {
                input.setAttribute('data-original-title', input.getAttribute('title'));
                input.removeAttribute('title');
            }

            input.addEventListener('invalid', function(e) {
                e.preventDefault();
                e.stopPropagation();
                e.stopImmediatePropagation();
                const name = this.getAttribute('name');
                if (validators[name]) {
                    const val = this.value;
                    const result = validators[name](val);
                    if (!result.valid) {
                        setInvalid(this, result.message);
                    }
                }
                return false;
            }, true);

            const name = input.getAttribute('name');
            let messageFieldFocused = name === 'Message' ? false : true;
            
            if (name === 'Message') {
                input.addEventListener('focus', function(e) {
                    messageFieldFocused = true;
                    const fieldName = this.getAttribute('name');
                    const val = this.value || '';
                    if (validators[fieldName]) {
                        const result = validators[fieldName](val);
                        if (result.valid) {
                            clearInvalid(this);
                        } else {
                            setInvalid(this, result.message);
                        }
                    }
                });
            }

            input.addEventListener('input', function(e) {
                const fieldName = this.getAttribute('name');
                
                if (fieldName === 'Message') {
                    if (messageFieldFocused) {
                        const val = this.value || '';
                        if (validators[fieldName]) {
                            const result = validators[fieldName](val);
                            if (result.valid) {
                                clearInvalid(this);
                            } else {
                                setInvalid(this, result.message);
                            }
                        }
                    }
                } else {
                    const val = sanitize(this.value);
                    this.value = val;
                    if (validators[fieldName]) {
                        const result = validators[fieldName](val);
                        if (result.valid) {
                            clearInvalid(this);
                        } else {
                            setInvalid(this, result.message);
                        }
                    }
                }
            });
            

            input.addEventListener('blur', function(e) {
                const fieldName = this.getAttribute('name');
                
                if (fieldName === 'Message') {

                    if (messageFieldFocused) {
                        const val = sanitizeMessage(this.value || '');
                        this.value = val;
                        if (validators[fieldName]) {
                            const result = validators[fieldName](val);
                            if (result.valid) {
                                clearInvalid(this);
                            } else {
                                setInvalid(this, result.message);
                            }
                        }
                    }
                } else {
                    const val = sanitize(this.value);
                    this.value = val;
                    if (validators[fieldName]) {
                        const result = validators[fieldName](val);
                        if (result.valid) {
                            clearInvalid(this);
                        } else {
                            setInvalid(this, result.message);
                        }
                    }
                }
            });
        });

        //--------------------------------------------------------------------------------------------------------
        // Set initial custom validity for required fields
        const allFields = form.querySelectorAll('input[name], textarea[name], select[name]');
        allFields.forEach(field => {
            const name = field.getAttribute('name');
            if (name && validators[name]) {
                const val = field.value;
                const result = validators[name](val);
                if (!result.valid) {
                    field.setCustomValidity(result.message);
                } else {
                    field.setCustomValidity('');
                }
            } else {
                field.setCustomValidity('');
            }
        });

        //-------------------------------------------------------------------------------------------------------
        // Handle form submission with validation
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            
            // Rate limiting check
            const now = Date.now();
            if (now - lastSubmissionTime < rateLimitWindow) {
                submissionAttempts++;
                if (submissionAttempts >= maxAttempts) {
                    showSecurityError('Too many submission attempts. Please wait 1 minute before trying again.');
                    return;
                }
            } else {
                submissionAttempts = 1;
                lastSubmissionTime = now;
            }
            
            let allValid = true;

            const fields = form.querySelectorAll('input[name], textarea[name], select[name]');
            fields.forEach(field => {
                const name = field.getAttribute('name');
                if (name === 'Message') {
                    field.value = sanitizeMessage(field.value);
                } else {
                    field.value = sanitize(field.value);
                }
                clearInvalid(field);
                field.setCustomValidity('');

                const val = field.value;
                
                if (validators[name]) {
                    const result = validators[name](val);
                    if (!result.valid) {
                        allValid = false;
                        setInvalid(field, result.message);
                    } else {
                        field.setCustomValidity('');
                    }
                } else {
                    field.setCustomValidity('');
                }
            });

            if (allValid) {
                form.submit();
            }
        });
    }

    //------------------------------------------------------------------------------------------------------------------
    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function() {
            setTimeout(initializeFormValidation, 100);
        });
    } else {
        setTimeout(initializeFormValidation, 100);
    }


    window.addEventListener('load', function() {
        setTimeout(initializeFormValidation, 200);
    });
})();
//-------------------------------------------------<<< Endof File >>>----------------------------------------------------
