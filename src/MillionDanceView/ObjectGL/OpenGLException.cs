using System;

namespace MillionDanceView.ObjectGL {
    public class OpenGLException : ApplicationException {

        public OpenGLException() {
        }

        public OpenGLException(string message)
            : base(message) {
        }

        public OpenGLException(string message, Exception innerException)
            : base(message, innerException) {
        }

    }
}
