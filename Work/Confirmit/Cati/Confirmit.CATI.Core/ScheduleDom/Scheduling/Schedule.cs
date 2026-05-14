using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using Confirmit.CATI.Core.ScheduleDom.Resources;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Confirmit.CATI.Common.ServiceLocation;
using Confirmit.CATI.Core.ScheduleDom.Script;
using Confirmit.CATI.Core.Services;

namespace Confirmit.CATI.Core.ScheduleDom.Scheduling
{
	/// <summary>
	/// Represents scheduling script. Scheduling script contains identifier, name, custom script
	/// and collections of rules, shifts, shift types and exclusion. This class does nothing, it is
	/// just a container.
	/// </summary>
	[Serializable]
	public class Schedule : BaseObject<int>
	{
	    private string m_name = String.Empty;
		private RuleCollection m_rules;
		private ShiftCollection m_shifts;
		private ShiftTypeCollection m_shiftTypes;
		private ExclusionCollection m_exclusions;
        private CustomParameterCollection m_customParameters;
        private CustomScript m_customScript;

	    /// <summary>
		/// Default constructor.
		/// </summary>
		public Schedule()
		{
			Rules = new RuleCollection();
			ShiftTypes = new ShiftTypeCollection();
			Shifts = new ShiftCollection();
			Exclusions = new ExclusionCollection();
            CustomParameters = new CustomParameterCollection();

			CustomScript = new CustomScript();
			CustomScript.Id = 1;
		}

		/// <summary>
		/// Protected copying constructor. 
		/// </summary>
		/// <param name="obj">Object to copy.</param>
		protected Schedule( Schedule obj )
		{
			if(obj == null)
			{
				throw new ArgumentNullException( "obj", Strings.ItemNullExceptionMessage );
			}

			Id = obj.Id;
			Name = obj.Name;
			CustomScript = obj.CustomScript;
			Rules = (RuleCollection)obj.Rules.Clone();
			Shifts = (ShiftCollection)obj.Shifts.Clone();
			ShiftTypes = (ShiftTypeCollection)obj.ShiftTypes.Clone();
			Exclusions = (ExclusionCollection)obj.Exclusions.Clone();
            CustomParameters = (CustomParameterCollection)obj.CustomParameters.Clone();
		}

	    /// <summary>
		/// Scheduling script name.
		/// </summary>
		[XmlElement]
		public string Name
		{
			get { return m_name ?? String.Empty; }
			set { m_name = value; }
		}

		/// <summary>
		/// Collection of rules.
		/// </summary>
		[XmlArray]
		public RuleCollection Rules
		{
			get { return m_rules; }
			set 
			{
				if(m_rules != null)
				{
					m_rules.Removing -= new RemovingEventHandler( OnRemovingRule );
				}

				m_rules = value;

				if(m_rules != null)
				{
					m_rules.Removing += new RemovingEventHandler( OnRemovingRule );
				}
			}
		}

		/// <summary>
		/// Collection of shift types.
		/// </summary>
		[XmlArray]
		public ShiftTypeCollection ShiftTypes
		{
			get { return m_shiftTypes; }
			set 
			{
				if(m_shiftTypes != null)
				{
					m_shiftTypes.Removing -=
						new RemovingEventHandler( OnRemovingShiftType );
				}

				m_shiftTypes = value;

				if(m_shiftTypes != null)
				{
					m_shiftTypes.Removing +=
							new RemovingEventHandler( OnRemovingShiftType );
				}
			}
		}

		/// <summary>
		/// Collection of shifts.
		/// </summary>
		[XmlArray]
		public ShiftCollection Shifts
		{
			get { return m_shifts; }
            set
            {
                if (m_shifts != null)
                {
                    m_shifts.Removing -=
                        new RemovingEventHandler(OnRemovingShift);
                }

                m_shifts = value;

                if (m_shifts != null)
                {
                    m_shifts.Removing +=
                            new RemovingEventHandler(OnRemovingShift);
                }
            }
		}

		/// <summary>
		/// Collection of exclusions.
		/// </summary>
		[XmlArray]
		public ExclusionCollection Exclusions
		{
			get { return m_exclusions; }
			set { m_exclusions = value; }
		}

        [XmlArray]
        public CustomParameterCollection CustomParameters
        {
            get { return m_customParameters; }
            set 
            {
                if (m_customParameters != null)
                {
                    m_customParameters.Removing -= new RemovingEventHandler(OnRemovingCustomParameter);
                }

                m_customParameters = value;

                if (m_customParameters != null)
                {
                    m_customParameters.Removing += new RemovingEventHandler(OnRemovingCustomParameter);
                }
            }
        }
		/// <summary>
		/// Custom script.
		/// </summary>
		[XmlElement]
		public CustomScript CustomScript
		{
			get { return m_customScript; }
			set { m_customScript = value; }
		}

	    /// <summary>
		/// Creates a new object that is a copy of the current instance. 
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		public override object Clone()
		{
			return new Schedule( this );
		}


	    /// <summary>
		/// Returns the number of rule or sub-rule according the position of item
		/// with given identifier in corresponding collection. The sub-rule number
		/// consists of two parts - the rule number and the sub-rule number separated
		/// with dot.
		/// </summary>
		/// <param name="id">Rule or sub-rule identifier.</param>
		/// <returns>Number as a string. If the item with given identifier
		/// doesn't exist, returns empty string.</returns>
		public string GetNumberByGuid( Guid id )
		{
			string result = String.Empty;

			Rule rule;
			for(int i = 0; i < Rules.Count; i++)
			{
				rule = Rules[i];

				if(rule.Id.Value == id)
				{
					result = (++i).ToString();
					break;
				}

				SubRule subRule;
				for(int j = 0; j < rule.SubRules.Count; j++)
				{
					subRule = rule.SubRules[j];

					if(subRule.Id.Value == id)
					{
						result = String.Format( "{0}.{1}", ++i, ++j );
						break;
					}
				}

				if(!String.IsNullOrEmpty( result ))
				{
					break;
				}
			}

			return result;
		}

		/// <summary>
		/// Returns the Guid identifier of rule or sub-rule by it's number.
		/// The number must be in format "number" or "number.number".
		/// </summary>
		/// <param name="number">Number of rule or sub-rule.</param>
		/// <returns>The identifier of object, if exists; otherwise empty Guid.</returns>
		/// <exception cref="FormatException">Object number is in wrong format.</exception>
		/// <exception cref="OverflowException">"number" represents a number less than Int32.MinValue 
		/// or greater than Int32.MaxValue.</exception>
		public Guid GetGuidByNumber( string number )
		{
			Guid result = Guid.Empty;
			Regex regEx = new Regex( @"^(?<rule_number>\d+)(\.(?<subrule_number>\d+))?$" );
			Match match = regEx.Match( number );
			if(match != Match.Empty)
			{
				int ruleIndex = Int32.Parse( match.Groups["rule_number"].Value ) - 1;
				if(ruleIndex >= 0 && ruleIndex < Rules.Count)
				{
					Rule rule = Rules[ruleIndex];
					Group subRuleGroup = match.Groups["subrule_number"];

					if(subRuleGroup.Success)
					{
						int subRuleIndex = Int32.Parse( subRuleGroup.Value ) - 1;
						if(subRuleIndex >= 0 && subRuleIndex < rule.SubRules.Count)
						{
							result = rule.SubRules[subRuleIndex].Id.Value;
						}
					}
					else
					{
						result = rule.Id.Value;
					}
				}
			}
			else
			{
				throw new FormatException( Strings.RuleNumberFormatExceptionMessage );
			}

			return result;
		}

		/// <summary>
		/// Returns array of timezone identifiers, which are used in current schedule object.
		/// </summary>
		/// <returns>Array of timezone identifiers.</returns>
		public int[] GetUsedTimezoneIds()
		{
			List<int> result = new List<int>();

			// collecting all timezone ids into temporary collection
			foreach(Shift shift in Shifts)
			{
				result.AddRange( shift.GetTimezoneIds() );
			}

			foreach(Exclusion exclusion in Exclusions)
			{
				result.AddRange( exclusion.GetTimezoneIds() );
			}

			if(result.Count > 0)
			{
				// sorting collection
				result.Sort();

				// removing duplicated values
				for(int i = result.Count - 2, cur = result[result.Count - 1]; i >= 0; i--)
				{
					if(cur == result[i])
					{
						result.RemoveAt( i );
					}
					else
					{
						cur = result[i];
					}
				}
			}

			return result.ToArray();
		}

		/// <summary>
		/// Rule removing event handler.
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="e">Event data.</param>
		protected void OnRemovingRule( object sender, RemovingEventArgs e )
		{
			Rule rule = Rules[e.Index];
			Guid ruleId = rule.Id.Value;
			string ruleNumber = GetNumberByGuid( ruleId );
			ErrorCollection errors = new ErrorCollection();

			Rule item;
			SubRule subRule;
			for(int i = 0; i < Rules.Count; i++ )
			{
				if(i == e.Index)
				{
					// do nor process the same rule
					continue;
				}

				item = Rules[i];
				
				for(int j = 0; j < item.SubRules.Count; j++ )
				{
					subRule = item.SubRules[j];

					foreach(SubRuleAction subRuleAction in subRule.SubRuleActions)
					{
						if(ActionManager.IsSetNextRuleAction( subRuleAction.ActionId.Value ))
						{
							try
							{
								/* we are trying to parse given action parameter as
								 * GUID. If fails we write log message
								 */
								Guid setNextRuleGuid = new Guid( GetActionParameterValue( subRuleAction) );
								if(ruleId == setNextRuleGuid)
								{
									errors.Add(
										new Error(
											String.Format(
												Strings.RuleIsInUseMessage,
												ruleNumber,
												i + 1,
												j + 1
											)
										)
									);
								}
							}
							catch(FormatException)
							{
                                Trace.TraceInformation(
									String.Format( 
										Strings.InvalidGuidStringMessage, 
										subRuleAction.Parameter 
									) 
								);		
							}
							catch(OverflowException)
							{
                                Trace.TraceInformation(
									String.Format(
										Strings.InvalidGuidStringMessage,
										subRuleAction.Parameter
									)
								);
							}
						}
					}
				}
			}

			e.Errors = errors;
			e.Cancel = (errors.Count != 0);
		}

        /// <summary>
        /// In this method we check if item is referenced by 
        /// another sub-rule in "Go to" action. If reference exists method returns 
        /// true and adds error to errors paremeter.
        /// </summary>
        public bool IsSubRuleUsed(Guid currentSubRuleID, out ErrorCollection errors)
        {
            errors = new ErrorCollection();
            for(int i = 0; i < Rules.Count; i++)
            {
                Rule rule = Rules[i];
                for(int j = 0; j < rule.SubRules.Count; j++)
                {
                    SubRule subRule = rule.SubRules[j];

                    if(subRule.Id == currentSubRuleID)
                    {
                        // do not process the same rule
                        continue;
                    }
                    foreach(SubRuleAction subRuleAction in subRule.SubRuleActions)
                    {
                        if(ActionManager.IsGoToAction(subRuleAction.ActionId.Value))
                        {
                            try
                            {
                                /* we are trying to parse given action parameter as
                                 * GUID. If fails we write log message
                                 */
                                Guid setNextRuleGuid = new Guid(GetActionParameterValue(subRuleAction));
                                if (currentSubRuleID == setNextRuleGuid)
                                {
                                    errors.Add(new Error(String.Format(
                                        Strings.SubRuleIsInUseMessage,
                                        GetNumberByGuid(currentSubRuleID),
                                        String.Format("{0}.{1}", i + 1, j + 1))));
                                }
                            }
                            catch(FormatException)
                            {
                                Trace.TraceInformation(
                                    String.Format(
                                        Strings.InvalidGuidStringMessage,
                                        subRuleAction.Parameter
                                        )
                                    );
                            }
                            catch(OverflowException)
                            {
                                Trace.TraceInformation(
                                    String.Format(
                                        Strings.InvalidGuidStringMessage,
                                        subRuleAction.Parameter
                                        )
                                    );
                            }
                        }
                    }
                }
            }

            return (errors.Count != 0);
        }

        protected void OnRemovingCustomParameter(object sender, RemovingEventArgs e)
        {
            CustomParameter customParameter = CustomParameters[e.Index];

            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            var actions = scheduleService.GetActions();
            
            foreach (SubRule subRule in Rules.SelectMany(x => x.SubRules))
            {
                foreach (SubRuleAction action in subRule.SubRuleActions)
                {
                    if (action.Parameter.Type == Parameter.ParamType.Parameter &&
                        action.Parameter.ParameterID == customParameter.Id)
                    {
                        Confirmit.CATI.Core.ScheduleDom.Script.Action tmp = actions.GetActionById(action.ActionId.Value);
                        e.Errors.Add(
                            new Error(
                                String.Format(
                                    Strings.CustomParameterIsInUseInActionMessage,
                                    customParameter.Name,
                                    tmp != null ? tmp.Name : action.ActionId.Value.ToString(),
                                    GetNumberByGuid(subRule.Id.Value)
                                )
                            )
                        );
                    }
                }
            }

            e.Cancel = (e.Errors.Count != 0);
        }

		/// <summary>
		/// Shift type removing event handler.
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="e">Event data.</param>
		protected void OnRemovingShiftType( object sender, RemovingEventArgs e )
		{
			ShiftType shiftType = ShiftTypes[e.Index];
			ErrorCollection errors = new ErrorCollection();
			if(shiftType.IsExclusionType)
			{
				Exclusions.ContainsItemsWithShiftType( shiftType, true, errors );
			}
			else
			{
				Shifts.ContainsItemsWithShiftType( shiftType, false, errors );
			}

            if (shiftType.IsExclusionType == false)
            {
                ContainsSubRulesWithShiftType(shiftType, errors);
                ContainsActionsWithShiftType(shiftType, errors);
            }

			e.Errors = errors;
			e.Cancel = (errors.Count != 0);
		}

        /// <summary>
		/// Shift removing event handler.
		/// </summary>
		/// <param name="sender">Sender of event.</param>
		/// <param name="e">Event data.</param>
        protected void OnRemovingShift(object sender, RemovingEventArgs e)
        {
            Shift shift = Shifts[e.Index];
            e.Errors = new ErrorCollection();

            var scheduleService = ServiceLocator.Resolve<IScheduleService>();
            var actions = scheduleService.GetActions();

            foreach(Rule rule in Rules)
            {
                foreach(SubRule subRule in rule.SubRules)
                {
                    foreach (SubRuleAction action in subRule.SubRuleActions)
                    {
                        if (action.ActionId.HasValue &&
                            ActionManager.IsRecallOnTheSpecificShift(action.ActionId.Value))
                        {
                            int shiftId;
                            if (Int32.TryParse(GetActionParameterValue( action ), out shiftId) &&
                                shiftId == shift.Id)
                            {
                                Confirmit.CATI.Core.ScheduleDom.Script.Action tmp = actions.GetActionById(action.ActionId.Value);
                                e.Errors.Add(
                                    new Error(
                                        String.Format(
                                            Strings.ShiftIsInUseInActionMessage, 
                                            shift.Id, 
                                            tmp != null ? tmp.Name : action.ActionId.Value.ToString(),
                                            GetNumberByGuid(subRule.Id.Value)
                                        )
                                    )
                                );
                            }
                        }
                    }
                }
            }

            e.Cancel = (e.Errors.Count != 0);
        }

		private bool ContainsSubRulesWithShiftType( ShiftType shiftType, ErrorCollection errors )
		{
			int oldCount = errors.Count;

			for(int i = 0; i < Rules.Count; i++)
			{
				Rule rule = Rules[i];
				for(int j = 0; j < rule.SubRules.Count; j++)
				{
					SubRule subRule = rule.SubRules[j];
					if(subRule.ShiftTypeId.HasValue && subRule.ShiftTypeId.Value == shiftType.Id.Value)
					{
						errors.Add( new Error( String.Format( Strings.ShiftTypeIsInUseMessage,
							shiftType.Name, "rule" , (i + 1).ToString() + "." + (j + 1).ToString() ) ) );
					}
				}
			}

			return (oldCount != errors.Count);
		}

        /// <summary>
        /// Determines whether schedule contains actions with the specified shift type.
        /// </summary>
        /// <param name="shiftType">Type of the shift.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>
        /// 	<c>true</c> if chedule contains actions with the specified shift type; otherwise, <c>false</c>.
        /// </returns>
        private bool ContainsActionsWithShiftType(ShiftType shiftType, ErrorCollection errors)
        {
            int oldCount = errors.Count;

            for (int i = 0; i < Rules.Count; i++)
            {
                Rule rule = Rules[i];
                for (int j = 0; j < rule.SubRules.Count; j++)
                {
                    SubRule subRule = rule.SubRules[j];
                    for (int k = 0; k < subRule.SubRuleActions.Count; k++)
                    {
                        SubRuleAction action = subRule.SubRuleActions[k];
                        if (ActionContainsShiftType(action, shiftType.Id))
                        {
                            errors.Add(new Error(String.Format(Strings.ShiftTypeIsInUseMessage,
                                                               shiftType.Name,
                                                               "action",
                                                               (i + 1) + "." + (j + 1) + "." + (k + 1))));
                        }
                    }
                }
            }

            return (oldCount != errors.Count);
        }

        private string GetActionParameterValue(SubRuleAction action)
        {
            switch (action.Parameter.Type)
            {
                case Parameter.ParamType.Constant:
                    return action.Parameter.Constant;
                case Parameter.ParamType.Parameter:
                    return CustomParameters.GetItemById(action.Parameter.ParameterID.Value).Value.ToString();
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Determines whether specified action contains shift type as parameter.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="shiftTypeID">The shift type ID.</param>
        /// <returns>
        /// 	<c>true</c> if specified action contains shift type as parameter; otherwise, <c>false</c>.
        /// </returns>
        private bool ActionContainsShiftType(SubRuleAction action, int? shiftTypeID)
        {
            int parameter;
            return action.ActionId.HasValue
                   && (ActionManager.IsSetShiftType(action.ActionId.Value)
                       || (ActionManager.IsRecallOnNextShiftOfSpecifiedType(action.ActionId.Value)))
                   && Int32.TryParse(GetActionParameterValue(action), out parameter)
                   && parameter == shiftTypeID;
        }
	}
}
