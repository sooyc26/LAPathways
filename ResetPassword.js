import React from 'react';
import { Modal, FormGroup, ControlLabel, FormControl, HelpBlock, Button } from 'react-bootstrap';
import * as validation from '../utils/validation';
import { connect } from 'react-redux';
import * as passwordResetService from '../services/passwordResetService';
import swal from 'sweetalert2'; 

class ResetPassword extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            show: false, 
            currentPassword: {
                input: false,
                value: ''
            },
            password: {
                input: false,
                value: ''
            }, 
            passwordConfirm: {
                input: false,
                value: ''
            }
        }

        this.onSave = this.onSave.bind(this);
        this.onChange = this.onChange.bind(this);
        this.hideModal = this.hideModal.bind(this); 
        this.onClose = this.onClose.bind(this);

    }

    hideModal(e) {
        e.preventDefault()
        this.setState({
            show: false
        })
    }

    onClose() {
        this.setState({
            show: false
        })
    }

    onChange(event) {
        const value = { value: event.target.value, input: true };
        this.setState({
            [event.target.name]: value
        })
    }

    checkValidation() {
        return validation.password(this.state.password.value) &&
            validation.confirmPassword(this.state.password.value, this.state.passwordConfirm.value)
    }

    onSave() {
        const newPassword = {
            password: this.state.password && this.state.password.value
        }
        const userId = this.props.userProfile.userId

        if (this.checkValidation()) {
            swal({
                title: 'Password has been reset',
                type: 'success'
            })
                .then(() => {
                    passwordResetService.edit(userId, newPassword)
                        .then(() => {
                            this.onClose()
                        })
                        .catch(console.log)
                })
        } else {
            swal({
                title: 'Invalid password',
                type: 'warning'
            })
        }
    }

    render() {
        const modalStyle = {
            position: 'fixed',
            zIndex: 2000,
            top: 0, bottom: 0, left: 0, right: 0
        };

        const backdropStyle = {
            ...modalStyle,
            zIndex: 'auto',
            backgroundColor: '#000',
            opacity: 0.5
        };

        return (
            <React.Fragment>
                <Button className="btn btn-light" type="submit" onClick={() => this.setState({ show: true })}>Reset Password</Button>

                <Modal
                    {...this.props}
                    bsSize="medium"
                    aria-labelledby="contained-modal-title-lg"
                    show={this.state.show}
                    animation={false}
                    backdropStyle={backdropStyle}
                    backdrop='static'
                >
                    <Modal.Header>

                        <div className="col-12">
                            <div style={{ float: 'right' }} onClick={e => this.hideModal(e)}>
                                <a href="" className="actions__item zmdi zmdi-close zmdi-hc-fw" ></a>
                            </div>
                        </div>
                    </Modal.Header>

                    <Modal.Body>
                    <h4>Reset Password</h4>
                        <br /><br />
                        <FormGroup>
                            <ControlLabel>Current Password</ControlLabel>
                            <FormControl
                                className={this.state.currentPassword.input && (validation.password(this.state.currentPassword.value) ? 'is-valid' : 'is-invalid')}
                                type="password"
                                name="currentPassword"
                                value={this.state.currentPassword.value}
                                onChange={this.onChange}
                            />
                            <i className="form-group__bar"></i>
                            <FormControl.Feedback />
                            {this.state.currentPassword.input && !validation.password(this.state.currentPassword.value) ? <HelpBlock style={{ position: "absolute" }}>Current password required</HelpBlock> : null}
                        </FormGroup>

                        <FormGroup>
                            <ControlLabel>New Password</ControlLabel>
                            <FormControl
                                className={this.state.password.input && (validation.password(this.state.password.value) ? 'is-valid' : 'is-invalid')}
                                type="password"
                                name="password"
                                value={this.state.password.value}
                                onChange={this.onChange}
                            />
                            <i className="form-group__bar"></i>
                            <FormControl.Feedback />
                            {this.state.password.input && !validation.password(this.state.password.value) ? <HelpBlock style={{ position: "absolute" }}>Please enter new password</HelpBlock> : null}
                        </FormGroup>

                        <FormGroup>
                            <ControlLabel>Confirm Password</ControlLabel>
                            <FormControl
                                className={this.state.passwordConfirm.input && (validation.confirmPassword(this.state.password.value, this.state.passwordConfirm.value) ? 'is-valid' : 'is-invalid')}
                                type="password"
                                name="passwordConfirm"
                                value={this.state.passwordConfirm.value}
                                onChange={this.onChange}
                            />
                            <i className="form-group__bar"></i>
                            <FormControl.Feedback />
                            {this.state.passwordConfirm.input && !validation.confirmPassword(this.state.password.value, this.state.passwordConfirm.value) ? <HelpBlock style={{ position: "absolute" }}>Password must match</HelpBlock> : null}
                        </FormGroup>
                    </Modal.Body>

                    <Modal.Footer>
                        <button class="btn btn-light" style={{ textAlign: "left" }} type="button" onClick={this.onSave}>Save Password</button>
                    </Modal.Footer>
                </Modal>
            </React.Fragment>

        )
    }
}

const mapStateToProps = state => ({
    userProfile: state.userProfiles
})

export default connect(mapStateToProps)(ResetPassword);

