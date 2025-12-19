using UnityEngine;

/// <summary>
/// Quiz Data ScriptableObject - stores quiz questions
/// </summary>
[CreateAssetMenu(fileName = "QuizData", menuName = "AR Lingo/Quiz Data")]
public class QuizData : ScriptableObject
{
    public QuizQuestion[] questions;
}