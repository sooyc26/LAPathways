import React, { Component } from 'react'
import * as surveyService from '../../../services/surveyService';
import { HelpBlock } from 'react-bootstrap';

export default class SurveyForm extends Component {
    constructor(props) {
        super(props);

        this.state = {
            id: '',
            name: '',
            description: '',
            statusId: '',
            ownerId: '',
            typeId: '',
            image: '',

            dataReceived: [],
            label: "Submit",
            edit: false,
            submitRdy: true
        };

        this.handleChange = this.handleChange.bind(this);
        this.onSubmit = this.onSubmit.bind(this);

        this.validation = this.validation.bind(this);
        this.validateName = this.validateName.bind(this);

        // this.updateById = this.updateById.bind(this);

    }

    handleChange(e) {
        this.setState({ [e.target.name]: e.target.value })
        console.log([e.target.name], e.target.value);
    }

    componentDidMount() {

        console.log("check match " + this.props.match.params.id);
        if (this.props.match.params.id) {
            surveyService.getById(this.props.match.params.id)
                .then((response) => {
                    const dataReceived = response.item;


                    this.setState({
                        id: dataReceived.id,
                        name: dataReceived.name,
                        description: dataReceived.description,
                        statusId: dataReceived.statusId,
                        ownerId: dataReceived.ownerId,
                        typeId: dataReceived.typeId,
                        label: "Edit",
                        edit: true
                    })
                    console.log("chck name " + (this.state.edit));
                })
        } else {
            this.setState({ edit: false })
        }
    }

    validateName(e) {

        var re = /[-!$%^&*()_+|~=`{}[:;<>?,.@#\]]/g;

        if (e !== '') {
            if (!re.test(e)) {
                return true;
            }
        }
        return false;
    }

    validation(e) {
        if (e !== "") {
            return true;;
        }
        return false;
    }

    validateStatus(e) {

        const id = parseInt(e);

        if (e !== '') {

            if (id === 0 || id === 1 || id === 2) {//if input not between 0 to 2
                return true;
            }
        }
        return false;
    }

    onSubmit() {

        const id = this.props.match.params.id;

        if (this.validation(this.state.ownerId) && this.validation(this.state.typeId) && this.validation(this.state.description)
            && this.validateName(this.state.name) && this.validateStatus(this.state.statusId)) {

            console.log("check input fields")

            this.setState({ submitRdy: true })

            const submitData = {
                // id: this.state.id,
                name: this.state.name,
                description: this.state.description,
                statusId: this.state.statusId,
                ownerId: this.state.ownerId,
                typeId: this.state.typeId
            };

            (this.state.edit ? surveyService.updateById(id, submitData) : surveyService.register(submitData))
                // .then("success register")
                .then(() => { this.props.history.push('/survey-list') })
                .catch(console.log.error);

        } else {
            this.setState({ submitRdy: false })
        }
    }

    render() {
        return (
            <React.Fragment>
                <header className="content__title">
                    <h1>Survey</h1>

                
                </header>

                {/* <SurveyUserInfo/> */}
                <div className="card">
                    <div className="card-body">
                        <form id="cardBody">

                            <h4 className="card-body__title">Name</h4>
                            <div className="form-group">
                                <input required type="text" name="name" value={this.state.name} onChange={this.handleChange} className={this.state.name === "" ? "form-control" : (this.validateName(this.state.name) ? "is-valid form-control" : "is-invalid form-control")} placeholder="Your Name" />
                                {/* <i className="form-group__bar"></i> */}
                                <HelpBlock> {this.validateName(this.state.name) ? "" : "invalid name"}</HelpBlock>

                            </div>
                            <h4 className="card-body__title">Description</h4>
                            <div className="form-group">
                                <input type="text" name="description" value={this.state.description} onChange={this.handleChange} className={this.validation(this.state.description) ? "is-valid form-control" : "form-control"} placeholder="Describe yourself!" />
                                {/* <textarea className="trumbowyg-textarea" tabindex="-1" style={{height: "260px"}}></textarea> */}
                                <i className="form-group__bar"></i>
                            </div>

                            <div className="row">

                                <div className="col-sm-3">
                                    <h3 className="card-body__title">Status Id</h3>
                                    <div className="form-group">
                                        <input required type="number" name="statusId" value={this.state.statusId} onChange={this.handleChange} className={this.state.statusId === "" ? "form-control" : (this.validateStatus(this.state.statusId) ? "is-valid form-control" : "is-invalid form-control")} placeholder="status id" />

                                        <HelpBlock> {this.validateStatus(this.state.statusId) ? "" : "numbers between 0-2"}</HelpBlock>
                                        {/* <i className="form-group__bar"></i> */}
                                    </div>
                                </div>

                                <div className="col-sm-3">
                                    <h3 className="card-body__title">owner Id</h3>
                                    <div className="form-group">
                                        <input type="number" name="ownerId" value={this.state.ownerId} onChange={this.handleChange} className={this.validation(this.state.ownerId) ? "is-valid form-control" : " form-control"} placeholder="owner id" />
                                        {/* <i className="form-group__bar"></i> */}
                                    </div>
                                </div>

                                <div className="col-sm-3">
                                    <h3 className="card-body__title">Type Id</h3>
                                    <div className="form-group">
                                        <input type="number" name="typeId" value={this.state.typeId} onChange={this.handleChange} className={this.validation(this.state.typeId) ? "is-valid form-control" : "form-control"} placeholder="type id" />
                                        {/* <i className="form-group__bar"></i> */}
                                    </div>
                                </div>
                                <span style={{ textAlign: "center", fontSize: 20 }}> {this.state.submitRdy ? "" : "Please enter valid Input!"} </span>
                                <button type="button" onClick={this.onSubmit} className="btn btn-light btn-block" style={{ float: 'right' }} >{this.state.label}</button>
                            </div>

                        </form>
                    </div>

                </div>
                <div className="card">
                    <div className="card-body">
                        <h4 className="card-title">Drag and drop image file upload!</h4>
                        <h6 className="card-subtitle">DropzoneJS is an open source library that provides drag’n’drop file uploads with image previews.</h6>

                        <form className="dropzone dz-clickable" id="dropzone-upload">
                            <div className="dz-default dz-message"><span>Drop files here to upload</span></div></form>
                    </div>
                </div>

            </React.Fragment >
        )
    }
}

// export {surveyDataGetAll}
