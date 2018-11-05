import React from 'react';

class Recommendtion extends React.Component{
    constructor(props) {
        super(props)

        this.submit = this.submit.bind(this)
    }

    submit(){
        this.props.history.push('/data-management/recommendation-logic')
    } 
    render() {
        return(
            <React.Fragment>
            <header className="content__title">
                    <h1>Recommendation</h1>
                </header>
                <div className="card col-md-12">
                    <div className="card-body col-md-12">
                    {this.props.location.state.answerRadio == 1 && <p>If the user selects "<em>{this.props.location.state.selectedAnswer.label}</em>" from the question "<em>{this.props.location.state.selectedQuestion.label}</em>"</p>}
                    {this.props.location.state.answerRadio == 0 && <p>If the user does NOT select "<em>{this.props.location.state.selectedAnswer.label}</em>" from the question "<em>{this.props.location.state.selectedQuestion.label}</em>"</p>}
                    {this.props.location.state.selectedAnswerTwo && <p>AND the user selects "<em>{this.props.location.state.selectedAnswerTwo.label}</em>" from the question "<em>{this.props.location.state.selectedQuestionTwo.label}</em>"</p>}
                    {this.props.location.state.selectedCoachExpertise && <React.Fragment><p>They will be matched with coaches who have <em>{this.props.location.state.selectedCoachExpertise.label}</em> expertise</p> <p>Reasoning: <em>{this.props.location.state.description.value}</em></p></React.Fragment>}
                    {this.props.location.state.selectedResource && <React.Fragment><p>They will be matched with resource providers from the <em>{this.props.location.state.selectedResource.label}</em> category.</p> <p>Reasoning: <em>{this.props.location.state.description.value}</em></p></React.Fragment>}
                    <button className="btn btn-light" onClick={this.submit}>Create More Recommendation Logic</button>
                    </div>
                    </div>
            </React.Fragment>
        )
    }
}

export default Recommendtion