using System;
using System.Collections.Generic;

namespace CDG.Chunks
{
	/// <summary>
	/// Encapsulates a Memory Preset CDG Chunk
	/// </summary>
    public class MemoryPreset : Chunk
	{
        public const int COLOUR_OFFSET = 0;
        public const int REPEAT_OFFSET = 1;

        // Validation constants
        const int MAX_COLOUR = 15;

		#region Construction

        public MemoryPreset(byte[] data)
            : base(data)
		{
            _ColourIndex = GetDataByte(COLOUR_OFFSET);
            _Repeat = GetDataByte(REPEAT_OFFSET);
		}

		#endregion

        #region Properties

        public int? ColourIndex
        {
            get
            {
                return _ColourIndex;
            }
            set
            {
                _ColourIndex = value;
                if (value.HasValue)
                {
                    SetDataByte(COLOUR_OFFSET, (byte)value);
                }
            }
        }

        public int? Repeat
        {
            get
            {
                return _Repeat;
            }
            set
            {
                _Repeat = value;
                if (value.HasValue)
                {
                    SetDataByte(REPEAT_OFFSET, (byte)value);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Execute(CDGBitmap bitmap)
        {
            base.Execute(bitmap);

            bitmap.BeginUpdate();
            bitmap.Clear(ColourIndex.Value);
            bitmap.HorizontalScrollOffset = 0;
            bitmap.VerticalScrollOffset = 0;
            bitmap.EndUpdate();
        }

        #endregion

        #region Validation

        public override List<CDG.Validation.Result> Validate(Validation.Rules rules)
        {
            List<CDG.Validation.Result> result = new List<CDG.Validation.Result>();

            // Simple checks
            int originalColour = ColourIndex.Value;
            if (originalColour > MAX_COLOUR)
            {
                result.Add(
                    new Validation.ChunkResult(this, 
                        string.Format("MemoryPreset: Invalid Colour Index {0}", originalColour)));
            }

            // Rule based checks
/*            if (!rules.BackgroundColours.Contains(originalColour))
            {
                bool repaired = false;

                if (rules.BackgroundColours.Count == 1)
                {
                    // There's only 1 valid background colour - so use that.
                    this.ColourIndex = rules.BackgroundColours[0];
                    repaired = true;
                }
                else
                {
                    // Check for consistency with next MemoryPreset command
                    Chunk chunk = FindNext(rules.File, Type);
                    if (chunk == null)
                    {
                        // Check for consistency with previous MemoryPreset command
                        chunk = FindPrevious(rules.File, Type);
                    }

                    if (chunk != null)
                    {
                        ColourIndex = (chunk as MemoryPreset).ColourIndex;
                        repaired = true;
                    }
                }
            
                if (repaired)
                {
                    Validation.MemoryPresetRepair repair = new Validation.MemoryPresetRepair(this, originalColour, ColourIndex);
                    repair.Status = Validation.Result.ResultStatus.Fixed;
                    result.Add(repair);
                }
            } */

            return result;
        }

        #endregion
        
        #region Object Overrides

		public override string ToString()
		{
			return "Memory Preset";
		}

		#endregion

        #region Data

        int? _ColourIndex = null;
        int? _Repeat = null;

        #endregion
    }
}
