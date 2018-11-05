import React from 'react';
import * as sendGridService from '../../../services/sendGridService'
import { withRouter } from 'react-router-dom';
import swal from 'sweetalert2'
import '../../layout/PageTransition.css'
import LoadingSpinner from '../../layout/LoadingSpinner';

class ResetPassword extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            id: '',
            loading: true,
            expiredText: 'Checking, please stand by!'
        }

        this.submitButton = this.submitButton.bind(this);
        this.onChange = this.onChange.bind(this);
        this.toRegister = this.toRegister.bind(this);
        this.toLogin = this.toLogin.bind(this);
        this.toResetPassword = this.toResetPassword.bind(this);
    }

    componentDidMount() {

        setTimeout(() => {
            sendGridService.expireDateCheck(this.props.match.params.key)
                .then((response) => {
                    console.log(response.item.expireBoolean)
                    this.setState({
                        check: response.item.expireBoolean,
                        id: response.item.id,
                        expiredText: "confirmed, directing to Password reset page!"
                    })
                })
                .then(() => {
                    setTimeout(() => {
                        this.props.history.push('/passwordreset/' + this.state.id);

                    }, 500);
                })
                .catch(() => {
                    this.setState({
                        loading: false,
                        expiredText: 'Key expired. Resend key?'
                    })
                });
        }, 2500)
    }

    submitButton() {
        const data = {
            email: this.state.email,
        }

        sendGridService.sendEmail(data)
            .then(() => swal({
                title:'Email Sent!',
                text: 'Please check your email.',
                type:'success',
                confirmButtonColor: '#7ac7f6',
                confirmButtonText: 'Close',
                background: '#0f2940'
            }))
            .then(() => this.toLogin())
            .catch(console.error)
        console.log("end");
    }

    onChange(e) {
        this.setState({ [e.target.name]: e.target.value })
        console.log(e.target.value)
    }

    toRegister() {
        this.props.history.push('/register');
    }

    toLogin() {
        this.props.history.push('/');
    }

    toResetPassword() {
        this.props.history.push('/resetPassword');
    }


    render() {
        const loginStyle = { position: 'absolute', top: '50%', left: '50%', marginTop: '-169px', marginLeft: '-165px', transform: 'translate(-50%, -50%)' }
        return (
            <div>

                {/* loading spinner */}
                {/* <div className={this.state.loading ? "page-loader" : "page-loader-hidden"} > */}
                <div style={{ position: "center" }} className={this.state.loading ? "page-loader__spinner" : "page-loader-hidden"} >
                    <svg viewBox="25 25 50 50">
                        <circle cx="50" cy="50" r="20" fill="none" strokeWidth="6" strokeMiterlimit="30" />
                    </svg>
                </div>
                {/* </div> */}

                <div className="login__block active" id="l-login" style={loginStyle}>
                    <div className="login__block__header">
                        <i className="zmdi zmdi-account-circle"></i>
                        CHECKING SECRET KEY EXPIRE DATE
                    <div className="actions actions--inverse login__block__actions">
                            <div className="dropdown">
                                <i data-toggle="dropdown" className="zmdi zmdi-more-vert actions__item"></i>

                                <div className="dropdown-menu dropdown-menu-right">
                                    <a onClick={this.toRegister} className="dropdown-item" data-sa-action="login-switch" data-sa-target="#l-register" href="">Don't have an account?</a>
                                    <a className="dropdown-item" onClick={this.toLogin} data-sa-action="login-switch" data-sa-target="#l-forget-password" href="">Remembered your password?</a>
                                    <a className="dropdown-item" onClick={this.toResetPassword} data-sa-action="login-switch" data-sa-target="#l-reset-password" href="">Reset password?</a>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div className="login__block__body">
                        <div className="form-group">
                            {this.state.expiredText}

                            {/* loading spinner */}
                            <LoadingSpinner loading={this.state.loading}></LoadingSpinner>

                        </div>

                    </div>
                </div>
            </div>
        )
    }
}

export default withRouter(ResetPassword)