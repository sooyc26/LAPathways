import React from 'react';
import Select from 'react-select';
import * as recommendationServices from '../../../services/recommendationsService';
import * as coachExpertiseServices from '../../../services/coachExpertiseService';
import * as resourceService from '../../../services/resourceService';
import { ControlLabel, FormControl, HelpBlock } from 'react-bootstrap';
import * as surveyQuestionAnswerOptionsService from '../../../services/SurveyQuestionAnswerOptionsService'

class RecommendationLogic extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            questionOptions: [
                { value: 'default', label: 'default' },
            ],

            selectedQuestion: '',
            answerOptions: [
                { value: 'default', label: 'default' },
            ],

            coachExpertiseOptions: [
                { value: 'default', label: 'default' },
            ],
            selectedCoachExpertise: null,
            resourceOptions: [
                { value: 'default', label: 'default' },
            ],
            selectedResource: null,
            description: {
                input: false,
                value: ''
            },
            id: '',
            whereString: '',
            answerOptionArr: [],
            questionArray: [],

            answerCheck:false,
            answer2Check:false,
            answerRadio:1,

            zipCheck: false,
            edit:false,

        }
                {/* ...removed for brevity */}

        this.toRecommendationList = this.toRecommendationList.bind(this);
        this.selectDropdowns = this.selectDropdowns.bind(this)
        this.extractAnswerOptionId = this.extractAnswerOptionId.bind(this)
        this.makeObj = this.makeObj.bind(this)
        this.setAnswers=this.setAnswers.bind(this)
        this.setQuestionsAndAnswers = this.setQuestionsAndAnswers.bind(this)
        this.reloadPage=this.reloadPage.bind(this)
    }

    componentDidMount() {
                      {/* ...removed for brevity */}


        Promise.all([prom1, prom2, prom3]).then(() => {

            if (this.props.match.params.id) {
                var id = this.props.match.params.id
                recommendationServices.readRecommendationById(id)
                    .then(response => {

                        var extract = this.extractAnswerOptionId(response.item.whereString)
                        this.setState({
                            id: response.item.id,
                            coeachExpertiseId: response.item.coachExpertiseTypeId,
                            resourceProviderId: response.item.resourceProviderId,
                            whereString: response.item.whereString,
                            description: this.makeObj(response.item.description),
                            answerOptionArr: extract.ansArr,
                            radio1: extract.radioArr[0],
                            radio2: extract.radioArr[1],
                            edit:true

                        })
                        return response.item
                    })
                    .then(response => {
                        this.selectDropdowns(response)
                        return this.state.answerOptionArr
                    })
                    .then(response => this.setQuestionsAndAnswers(response))

            }
        })
    }

    makeObj(val) {
        const obj = {
            input: true,
            value: val
        }
        return obj;
    }

 setQuestionsAndAnswers(answerArray) {
     
        if (answerArray.length === 2) {
            const promise1 = surveyQuestionAnswerOptionsService.surveyQuestionAnswerOptionsReadById(answerArray[0])
                .then(response => {  
                    const questionSelection = this.state.questionOptions.find(option => option.value === response.item.questionId)
                    this.setState({
                        selectedQuestion: questionSelection
                    })                    
                    return response.item.questionId
                })

            const promise2 = surveyQuestionAnswerOptionsService.surveyQuestionAnswerOptionsReadById(answerArray[1])
                .then(response => {
                     
                    const questionSelection2 = this.state.questionTwoOptions.find(option => option.value === response.item.questionId)
                    this.setState({
                        selectedQuestionTwo: questionSelection2
                    })                     
                    return response.item.questionId
                })

            const returnPromise = Promise.all([promise1, promise2])
                .then(([prom1, prom2]) => {
                    if (this.state.selectedQuestionTwo) {
                        const promise11 = this.getAnswerOptionsTwo(this.state.selectedQuestionTwo)
                        const promise10 = this.getAnswerOptionsOne(this.state.selectedQuestion)
                        return Promise.all([promise10, promise11])
                    }
                })

            return returnPromise;

        } else {
            const promise1 =  surveyQuestionAnswerOptionsService.surveyQuestionAnswerOptionsReadById(answerArray[0])
                .then(response => {
                    const questionSelection = this.state.questionOptions.find(option => option.value === response.item.questionId)
                    this.setState({
                        selectedQuestion: questionSelection
                    })
                    return response.item.questionId
                })
                .then((prom1) => {
                    const promise10 = this.getAnswerOptionsOne(this.state.selectedQuestion)
                })
                
            return promise1;
        }
    }

    extractAnswerOptionId(whereString) {
        let radioArr = [];
        let extract = whereString.replace(/#TempSurveyAnswers.AO_/g, '');
        let strArray = extract.split('AND')
        let ansArr = [];
        if(strArray.includes('IN')){
            this.setState({
                zipCheck:true
            })
         
        }
        for (let i = 0; i < strArray.length; i++) {
            if (strArray[i].includes('= 1') || strArray[i].includes('=1')) {
                radioArr.push(1)
                strArray[i] = strArray[i].replace(/= 1/, '');
            } else {

                radioArr.push(0)
                strArray[i] = strArray[i].replace(/= 0/, '');
            }
            ansArr.push(parseInt(strArray[i]))
        }

        if(ansArr.length===2){
            this.newQuestion()          
        }
        return { ansArr, radioArr }
    }

    selectDropdowns(response) {
        let coachSelection = this.state.coachExpertiseOptions.find(option => option.value === response.coachExpertiseTypeId)
        let resourceSelection = this.state.resourceOptions.find(option => option.value === response.resourceProviderId)

        this.setState({
            selectedCoachExpertise: coachSelection,
            selectedResource: resourceSelection,
        });
    }

    setAnswers(answerArray) {
        let answerSelection2 = ''
        if (this.state.answerOptionArr.length === 2) {
            answerSelection2 = this.state.answerTwoOptions.find(option => option.value === answerArray[1])
            this.setState({
                selectedAnswerTwo: answerSelection2,
                answerCheck2: false
            });
            let answerSelection = this.state.answerOptions.find(option => option.value === answerArray[0])
            this.setState({
                selectedAnswer: answerSelection
                , answerCheck: false
            });
        } else {
            let answerSelection = this.state.answerOptions.find(option => option.value === answerArray[0])
            this.setState({
                selectedAnswer: answerSelection
                , answerCheck: false
            });
        }
    }

                {/* ...removed for brevity */}


    submit() {
        const data = this.state;
        
        if (this.props.match.params.id) {
            let id = this.props.match.params.id
            recommendationServices.put(this.state, id)
            .then(() => {
                this.props.history.push("/data-management/recommendation/list/");
            })
        }else{
        recommendationServices.post(data)
            .then(() => {
                this.props.history.push("/data-management/recommendation/list/");
            })
        }
    }

    toRecommendationList() {
        this.props.history.push("/data-management/recommendation/list/");
    }
    reloadPage(){
        this.props.history.push("/data-management/recommendation-logic/");
    }

    render() {
        if ((this.state.answerCheck || this.state.answerCheck2)&&this.state.edit) {
            this.setAnswers(this.state.answerOptionArr);
        }

        const customStyle = {
            option: (base, state) => ({
                borderBottom: '1px dotted gray',
                padding: 8,
                ...base,
                color: state.isFocused ? 'blue' : 'black',
            }),
        }
        return (
                          {/* ...removed for brevity */}

        )
    }
}

export default RecommendationLogic
